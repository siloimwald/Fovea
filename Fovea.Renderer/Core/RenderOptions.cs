namespace Fovea.Renderer.Core;

public class RenderOptions
{
    public int NumSamples { get; set; } = 200;
    public int ImageWidth { get; set; } = 800;
    public int ImageHeight { get; set; } = 600;
    public int MaxDepth { get; init; } = 50;
    public string OutputFile { get; set; } = "output.png";
}