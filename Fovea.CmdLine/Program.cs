using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Yaml;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Diagnostics;

namespace Fovea.CmdLine;

internal class Program
{
    private static readonly ILogger<Program> Log = Logging.GetLogger<Program>();
    
    private static void Main(string[] args)
    {

        if (args.Length > 0)
        {
            Log.LogInformation("reading scene file {FileName}", args[0]);
            var scene = YamlParser.ParseFile(args[0]);
            // var scene = DemoSceneCreator.GetFinalSceneBookOne();
            var renderer = new Raytracer();
            renderer.Render(scene);
        }
        
            
        
        // TODO: need to dispose all the stuff...
        Console.WriteLine(MemoryDiagnostics.TotalUndisposedAllocationCount);
    }
}