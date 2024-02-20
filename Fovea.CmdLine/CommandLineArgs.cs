using CommandLine;

namespace Fovea.CmdLine;

/// <summary>
/// command line args to allow certain parameters being overriden through the cli
/// </summary>
public class CommandLineArgs
{
    [Option('s', Required = true, HelpText = "json input scene file")]
    public string SceneFile { get; set; }
    [Option('n', Required = false, HelpText = "number of samples override")]
    public int NumSamples { get; set; }
    [Option('w', Required = false, HelpText = "image width override")]
    public int ImageWidth { get; set; }
    [Option('h', Required = false, HelpText = "image height override")]
    public int ImageHeight { get; set; }

    [Option('f', Required = false, HelpText = "filename override")]
    public string OutputFilename { get; set; } = string.Empty;
}