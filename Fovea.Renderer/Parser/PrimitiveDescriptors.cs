
namespace Fovea.Renderer.Parser;

public abstract class PrimitiveDescriptorBase
{
    public string Material { get; init; } = string.Empty;
}

/// <summary>
/// axis aligned quad (producing two triangles)
/// </summary>
public class QuadDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector2 TopLeft { get; init; }
    public Vector2 BottomRight { get; init; }

    public string Axis { get; init; } = "Y";
}

/// <summary>
/// a sphere
/// </summary>
public class SphereDescriptor : PrimitiveDescriptorBase, IPrimitiveGenerator
{
    public Vector3 Center { get; init; }
    public float Radius { get; init; }
}