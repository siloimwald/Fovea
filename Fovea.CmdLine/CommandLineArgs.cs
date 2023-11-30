using CommandLine;

namespace Fovea.CmdLine;

public class CommandLineArgs
{
    [Option('s', Required = false, HelpText = "number of samples", Default = 50)]
    public int NumSamples { get; set; }

    [Option('w', Required = false, HelpText = "image width, height is determined by aspect ratio", Default = 400)]
    public int ImageWidth { get; set; }
}