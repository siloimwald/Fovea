using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Parser;

public class CameraDescriptor
{
    public float FieldOfView { get; init; } = 45f;
    public float Near { get; init; } = 1f;
    public float Far { get; init; } = 1000f;

    public Orientation Orientation { get; init; } = new()
    {
        LookAt = Vector3.Zero,
        LookFrom = Vector3.UnitZ,
        UpDirection = Vector3.UnitY
    };

    // for the time being, this will do...
    public PerspectiveCamera AsPerspectiveCamera(float aspectRatio)
    {
        return new PerspectiveCamera(Orientation, aspectRatio, FieldOfView, Near, Far);
    }
}