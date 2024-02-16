using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Yaml;
using SixLabors.ImageSharp.Diagnostics;

namespace Fovea.CmdLine;

internal class Program
{
    private static void Main(string[] args)
    {

        if (args.Length > 0)
        {
            Console.WriteLine($"reading scene file {args[0]}");
            var scene = YamlParser.ParseFile(args[0]);
            var renderer = new Raytracer();
            renderer.Render(scene);
        }
        
            
        
        // TODO: need to dispose all the stuff...
        Console.WriteLine(MemoryDiagnostics.TotalUndisposedAllocationCount);
    }
}