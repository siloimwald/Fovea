using System;
using Fovea.Renderer.Core;
using CommandLine;
using Fovea.Renderer.Parser;

namespace Fovea.CmdLine
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArgs>(args)
                .WithParsed(opts =>
                {
                    var renderer = new Raytracer
                    {
                        NumSamples = opts.NumSamples
                    };
                    var scene = DemoSceneCreator.MakeScene(DemoScenes.CylinderTest, opts.ImageWidth);
                    renderer.Render(scene);
                });
            
        }
    }
}