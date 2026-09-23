using UnityEngine;

public class PassthroughManager : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer; // Public reference

    public void SetPassthroughDimmed(bool dimmed)
    {
        if (passthroughLayer == null) return;
        
        passthroughLayer.colorMapEditorType = OVRPassthroughLayer.ColorMapEditorType.ColorAdjustment;
        float brightness = dimmed ? -0.8f : 0.0f; 
        float contrast = dimmed ? -0.2f : 0.0f;
        
        passthroughLayer.SetBrightnessContrastSaturation(brightness, contrast, 0f);
    }
}