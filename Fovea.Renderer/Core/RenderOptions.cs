namespace Fovea.Renderer.Core;

public class RenderOptions
{
    public int NumSamples { get; init; } = 200;
    public int ImageWidth { get; init; } = 800;
    public int ImageHeight { get; init; } = 600;
    public int MaxDepth { get; init; } = 50;
    public string OutputFile { get; init; } = "output.png";
}