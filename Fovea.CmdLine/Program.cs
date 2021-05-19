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
                        NumSamples = opts.NumSamples,
                        ImageWidth = opts.ImageWidth
                    };
                    renderer.Render();        
                });
            
        }
    }
}