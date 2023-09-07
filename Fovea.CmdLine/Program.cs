using System;
using Fovea.Renderer.Core;

namespace Fovea.CmdLine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("rendering all test scenes...");
            foreach (int i in Enum.GetValues(typeof(DemoScenes)))
            {
                var fileName = $"{Enum.GetName(typeof(DemoScenes), i)}.ppm";
                Console.WriteLine($"rendering {fileName}");
                var renderer = new Raytracer
                {
                    NumSamples = 10
                };
                var scene = DemoSceneCreator.MakeScene((DemoScenes)i, 512);
                renderer.Render(scene, fileName);
            }
            
            // Parser.Default.ParseArguments<CommandLineArgs>(args)
            //     .WithParsed(opts =>
            //     {
            //         var renderer = new Raytracer
            //         {
            //             NumSamples = opts.NumSamples
            //         };
            //         var foo = DemoScenes.GetValuesAsUnderlyingType<int>()
            //         
            //         var scene = DemoSceneCreator.MakeScene(DemoScenes.FinalSceneBookTwo, opts.ImageWidth);
            //         renderer.Render(scene);
            //     });
        }
    }
}