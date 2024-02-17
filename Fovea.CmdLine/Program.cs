using System;
using CommandLine;
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
        Parser.Default.ParseArguments<CommandLineArgs>(args)
            .WithParsed(opts =>
            {
                Log.LogInformation("reading scene file {FileName}", opts.SceneFile);
                var scene = YamlParser.ParseFile(opts.SceneFile);
                var renderer = new Raytracer();
                
                // not great, but does the job. allow optional parameters to override
                // certain parameters to be overruled. mostly those that affect render speed
                // for easier testing
                
                if (opts.NumSamples > 0)
                {
                    Log.LogInformation("override sample count new={newSampleCount} old=({oldSampleCount})",
                        opts.NumSamples, scene.Options.NumSamples);
                    scene.Options.NumSamples = opts.NumSamples;
                }

                if (opts.ImageWidth > 0)
                {
                    Log.LogInformation("override image width new={newImageWidth} old=({oldImageWidth})",
                        opts.ImageWidth, scene.Options.ImageWidth);
                    scene.Options.ImageWidth = opts.ImageWidth;
                }

                if (opts.ImageHeight > 0)
                {
                    Log.LogInformation("override image height new={newImageHeight} old=({oldImageHeight})",
                        opts.ImageHeight, scene.Options.ImageHeight);
                    scene.Options.ImageHeight = opts.ImageHeight;
                }

                if (!string.IsNullOrEmpty(opts.OutputFilename))
                {
                    Log.LogInformation("override output filename new={newOutputFileName} old=({oldOutputFileName})",
                        opts.OutputFilename, scene.Options.OutputFile);
                    scene.Options.OutputFile = opts.OutputFilename;   
                }
                
                renderer.Render(scene);
            });
        
        Log.LogDebug("not disposed image sharp allocations {nonDisposedAllocations}",
            MemoryDiagnostics.TotalUndisposedAllocationCount);
    }
}