using System;
using Fovea.Renderer.Core;
using CommandLine;

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
                    var scene = DemoSceneCreator.MakeScene(DemoScenes.BoxCSGTest, opts.ImageWidth);
                    renderer.Render(scene);        
                });
            
        }
    }
}