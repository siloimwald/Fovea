using System;
using CommandLine;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Parser.Json;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Diagnostics;

namespace Fovea.CmdLine;

internal class Program
{
    private static readonly ILogger<Program> Log = Logging.GetLogger<Program>();

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandLineArgs>(args)
            .WithParsed(opts =>
            {
                Log.LogInformation("reading scene file {FileName}", opts.SceneFile);

                try
                {
                    var optionOverrides = new OptionsOverride
                    {
                        ImageHeight = opts.ImageHeight,
                        ImageWidth = opts.ImageWidth,
                        OutputFile = opts.OutputFilename,
                        NumSamples = opts.NumSamples
                    };
                    
                    var scene = JsonParser.ParseFile(opts.SceneFile, optionOverrides);
                    var renderer = new Raytracer();
                    renderer.Render(scene);
                }
                catch (Exception err)
                {
                    Log.LogInformation("rendering failed due to error: {Message}", err.Message);
                }
            });

        if (MemoryDiagnostics.TotalUndisposedAllocationCount > 0)
        {
            Log.LogWarning("not disposed image sharp allocations {nonDisposedAllocations}",
                MemoryDiagnostics.TotalUndisposedAllocationCount);
        }
    }
}