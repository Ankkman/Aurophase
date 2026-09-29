using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction.Input;

public class PlanetPalmRotation : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private Transform planet;
    [SerializeField, Tooltip("Required for accurate surface distance. If null, it will find one automatically.")] 
    private Collider planetCollider;

    [Header("Palm Rotation Settings")]
    [SerializeField, Tooltip("Distance from the planet's SURFACE to trigger rotation")] 
    private float surfaceInteractionDistance = 0.15f; 
    [SerializeField] private float rotationSensitivity = 1.0f;
    [SerializeField, Tooltip("Prevents tiny planets from spinning uncontrollably fast")] 
    private float minimumVirtualRadius = 0.15f;
    [SerializeField] private bool invertRotation = false;

    [Header("Smoothing & Stability")]
    [SerializeField] private float smoothing = 15f;
    [SerializeField] private float deadZoneAngle = 0.5f;

    [Header("Inertia (Momentum)")]
    [SerializeField] private bool enableInertia = true;
    [SerializeField, Tooltip("Higher value = stops faster")] 
    private float spinDecay = 3f;

    [Header("Feedback Events")]
    [SerializeField, Tooltip("Degrees of rotation before triggering a tick")] 
    private float tickAngleThreshold = 10f;
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;
    public UnityEvent OnSpinTick;

    private HandRef leftHand;
    private HandRef rightHand;

    private Vector3 previousPalmPositionLeft;
    private Vector3 previousPalmPositionRight;

    private bool leftActive;
    private bool rightActive;
    private bool wasHoveringLastFrame;

    private Quaternion targetRotation;
    
    // Inertia & Feedback tracking
    private Vector3 inertiaAxis;
    private float inertiaAngle;
    private float accumulatedAngleForTick;

    private void Awake()
    {
        if (planet == null) planet = transform;
        if (planetCollider == null) planetCollider = planet.GetComponentInChildren<Collider>();
        
        targetRotation = planet.rotation;
    }

    private void Start()
    {
        FindHands();
    }

    private void FindHands()
    {
        HandRef[] hands = FindObjectsByType<HandRef>(FindObjectsSortMode.None);
        foreach (HandRef hand in hands)
        {
            if (hand.Handedness == Handedness.Left) leftHand = hand;
            if (hand.Handedness == Handedness.Right) rightHand = hand;
        }
        
        if (leftHand == null) Debug.LogWarning($"{name}: Left HandRef not found.");
        if (rightHand == null) Debug.LogWarning($"{name}: Right HandRef not found.");
    }

    private void Update()
    {
        if (planet == null) return;

        // Process hands (Left takes priority over Right)
        bool leftInteracted = ProcessHand(leftHand, ref previousPalmPositionLeft, ref leftActive);
        if (!leftInteracted)
        {
            ProcessHand(rightHand, ref previousPalmPositionRight, ref rightActive);
        }
        else
        {
            rightActive = false;
        }

        bool isHoveringNow = leftActive || rightActive;

        // Handle Hover Events for visual affordances (auras/glows)
        if (isHoveringNow && !wasHoveringLastFrame)
        {
            OnHoverEnter?.Invoke();
        }
        else if (!isHoveringNow && wasHoveringLastFrame)
        {
            OnHoverExit?.Invoke();
        }
        wasHoveringLastFrame = isHoveringNow;

        // Apply Inertia when not actively interacting
        if (!isHoveringNow && enableInertia && inertiaAngle > 0.01f)
        {
            Quaternion appliedRotation = Quaternion.AngleAxis(inertiaAngle, inertiaAxis);
            targetRotation = appliedRotation * targetRotation;
            
            accumulatedAngleForTick += inertiaAngle;
            CheckForTick();

            inertiaAngle = Mathf.Lerp(inertiaAngle, 0f, spinDecay * Time.deltaTime);
        }

        // Apply smoothed rotation continuously
        planet.rotation = Quaternion.Slerp(planet.rotation, targetRotation, smoothing * Time.deltaTime);
    }

    private bool ProcessHand(HandRef hand, ref Vector3 previousPalmPos, ref bool wasActive)
    {
        if (hand == null || !hand.IsTrackedDataValid)
        {
            wasActive = false;
            return false;
        }

        if (!hand.GetJointPose(HandJointId.HandPalm, out Pose palmPose))
        {
            wasActive = false;
            return false;
        }

        // Yield to Meta's Grab system. Kill inertia and sync rotation.
        if (hand.GetIndexFingerIsPinching())
        {
            wasActive = false;
            targetRotation = planet.rotation;
            inertiaAngle = 0f; 
            return false;
        }

        // Calculate distance strictly from the SURFACE of the planet
        float distanceToSurface;
        Vector3 surfacePoint;
        
        if (planetCollider != null)
        {
            surfacePoint = planetCollider.ClosestPoint(palmPose.position);
            distanceToSurface = Vector3.Distance(palmPose.position, surfacePoint);
        }
        else
        {
            // Fallback if no collider exists
            float estimatedRadius = planet.lossyScale.x * 0.5f;
            distanceToSurface = Mathf.Max(0f, Vector3.Distance(palmPose.position, planet.position) - estimatedRadius);
            surfacePoint = planet.position + (palmPose.position - planet.position).normalized * estimatedRadius;
        }

        if (distanceToSurface > surfaceInteractionDistance)
        {
            wasActive = false;
            return false;
        }

        Vector3 currentPalmPos = palmPose.position;

        if (!wasActive)
        {
            previousPalmPos = currentPalmPos;
            targetRotation = planet.rotation;
            wasActive = true;
            return true;
        }

        // --- TRANSLATION-TO-ROTATION TRACKBALL ---
        Vector3 deltaPos = currentPalmPos - previousPalmPos;
        
        // Vector from center of planet to the hand
        Vector3 centerToHand = (currentPalmPos - planet.position).normalized;
        
        // Cross product gives the correct rolling rotation axis
        Vector3 axis = Vector3.Cross(centerToHand, deltaPos);
        
        if (axis == Vector3.zero) return true; // Prevent NaN errors if hand moves directly away
        axis.Normalize();

        // Determine radius for math (enforce a minimum so tiny planets don't mathematically explode)
        float actualRadius = Vector3.Distance(planet.position, surfacePoint);
        float effectiveRadius = Mathf.Max(actualRadius, minimumVirtualRadius);

        // Arc length formula: angle in radians = arc_length / radius
        float angle = (deltaPos.magnitude / effectiveRadius) * Mathf.Rad2Deg;

        if (angle > deadZoneAngle)
        {
            angle *= rotationSensitivity;
            if (invertRotation) angle *= -1f;

            Quaternion appliedRotation = Quaternion.AngleAxis(angle, axis);
            targetRotation = appliedRotation * targetRotation;
            
            // Save data for Inertia
            inertiaAxis = axis;
            inertiaAngle = Mathf.Abs(angle);

            accumulatedAngleForTick += inertiaAngle;
            CheckForTick();

            // Only update anchor when deadzone is broken (eliminates tracking micro-jitter)
            previousPalmPos = currentPalmPos; 
        }
        else
        {
            inertiaAngle = 0f;
        }

        return true;
    }

    private void CheckForTick()
    {
        if (accumulatedAngleForTick >= tickAngleThreshold)
        {
            OnSpinTick?.Invoke();
            accumulatedAngleForTick -= tickAngleThreshold;
        }
    }
}