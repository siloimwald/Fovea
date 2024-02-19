using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Parser;

/// <summary>
/// book 1 level currently
/// </summary>
public class CameraDescriptor
{
    public float FieldOfView { get; init; } = 45f;
    // Distance from camera look from point to plane of perfect focus
    public float FocusDistance { get; init; } = 10.0f;
    // Variation angle of rays through each pixel
    public float DefocusAngle { get; init; } = 0.0f;
    
    public Orientation Orientation { get; init; } = new()
    {
        LookAt = Vector3.Zero,
        LookFrom = Vector3.UnitZ,
        UpDirection = Vector3.UnitY
    };
    
}