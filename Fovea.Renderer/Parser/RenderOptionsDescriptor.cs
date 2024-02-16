using YamlDotNet.Serialization;

namespace Fovea.Renderer.Parser;

public class RenderOptionsDescriptor
{
    public int NumSamples { get; init; } = 200;
    public int ImageWidth { get; init; } = 800;
    public int ImageHeight { get; init; } = 600;
    public int MaxDepth { get; init; } = 50;
}