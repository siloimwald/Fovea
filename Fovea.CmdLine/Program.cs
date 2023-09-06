using CommandLine;
using Fovea.Renderer.Core;

namespace Fovea.CmdLine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArgs>(args)
                .WithParsed(opts =>
                {
                    var renderer = new Raytracer
                    {
                        NumSamples = opts.NumSamples
                    };
                    var scene = DemoSceneCreator.MakeScene(DemoScenes.FinalSceneBookTwo, opts.ImageWidth);
                    renderer.Render(scene);
                });
        }
    }
}