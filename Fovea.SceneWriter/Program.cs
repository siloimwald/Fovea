// serialize demo and book scenes to yaml, mostly for scenes that do contain randomized elements

using System.Diagnostics;
using CommandLine;
using Fovea.Renderer.Parser.Yaml;
using Fovea.SceneWriter;

Parser.Default.ParseArguments<CommandLineArgs>(args)
    .WithParsed(opts =>
    {
        try
        {
            var sceneDescriptor = DemoSceneCreator.MakeScene(opts.SceneId);
            var serializer = YamlParser.GetSerializer();
            var yamlText = serializer.Serialize(sceneDescriptor);
            File.WriteAllText(opts.OutputFile, yamlText);
            Debug.WriteLine($"wrote scene to {opts.OutputFile}");
        }
        catch (Exception err)
        {
            Debug.WriteLine($"something went wrong {err}");
        }
    });