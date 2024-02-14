using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Parser;

public class CameraDescriptor
{
    public float FieldOfView { get; init; }
    public float Near { get; init; }
    public float Far { get; init; }
    public Orientation Orientation { get; init; }
}