namespace Fovea.Renderer.Core;

public class RenderOptions
{
    public int NumSamples { get; set; } = 200;
    public int ImageWidth { get; set; } = 800;
    public int ImageHeight { get; set; } = 600;
    public int MaxDepth { get; init; } = 50;
    public string OutputFile { get; set; } = "output.png";

    public override string ToString() =>
        $"Render Options: Sample Count = {NumSamples}, Image Size {ImageWidth}x{ImageHeight}, Output File = {OutputFile}";
}