using CommandLine;

namespace Fovea.SceneWriter;

public class CommandLineArgs
{
    [Option('s', Required = true, HelpText = "scene id")]
    public DemoSceneId SceneId { get; set; }

    [Option('f', Required = true, HelpText = "output yaml file")]
    public string OutputFile { get; set; } = string.Empty;
}