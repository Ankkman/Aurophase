using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction.Input;

public class PlanetPalmRotation : MonoBehaviour
{
    [Header("Planet")]
    [SerializeField] private Transform planet;

    [Header("Palm Rotation Settings")]
    [SerializeField] private float interactionDistance = 0.35f;
    [SerializeField] private float rotationSensitivity = 1.0f;
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

    private Vector3 previousHitDirectionLeft;
    private Vector3 previousHitDirectionRight;

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
        bool leftInteracted = ProcessHand(leftHand, ref previousHitDirectionLeft, ref leftActive);
        if (!leftInteracted)
        {
            ProcessHand(rightHand, ref previousHitDirectionRight, ref rightActive);
        }
        else
        {
            rightActive = false; // Reset right if left is active
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
            
            // Track ticks during inertia
            accumulatedAngleForTick += inertiaAngle;
            CheckForTick();

            // Decay the spin speed over time
            inertiaAngle = Mathf.Lerp(inertiaAngle, 0f, spinDecay * Time.deltaTime);
        }

        // Apply smoothed rotation continuously
        planet.rotation = Quaternion.Slerp(planet.rotation, targetRotation, smoothing * Time.deltaTime);
    }

    private bool ProcessHand(HandRef hand, ref Vector3 previousDirection, ref bool wasActive)
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

        float distance = Vector3.Distance(palmPose.position, planet.position);
        if (distance > interactionDistance)
        {
            wasActive = false;
            return false;
        }

        Vector3 currentDirection = (palmPose.position - planet.position).normalized;

        if (!wasActive)
        {
            previousDirection = currentDirection;
            targetRotation = planet.rotation;
            wasActive = true;
            return true;
        }

        Quaternion deltaRotation = Quaternion.FromToRotation(previousDirection, currentDirection);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        // Apply rotation if deadzone is broken
        if (Mathf.Abs(angle) > deadZoneAngle)
        {
            angle *= rotationSensitivity;
            if (invertRotation) angle *= -1f;

            Quaternion appliedRotation = Quaternion.AngleAxis(angle, axis);
            targetRotation = appliedRotation * targetRotation;
            
            // Save data for Inertia/Momentum
            inertiaAxis = axis;
            inertiaAngle = Mathf.Abs(angle);

            // Accumulate angle for Audio Feedback
            accumulatedAngleForTick += inertiaAngle;
            CheckForTick();

            previousDirection = currentDirection; 
        }
        else
        {
            // If hand stops moving, kill inertia so it doesn't spin wildly upon release
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