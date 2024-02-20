﻿using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Viewing;
using Microsoft.Extensions.Logging;

namespace Fovea.Renderer.Parser;

/// <summary>
/// scene descriptor is the parent parser node for our json scene file
/// and sticks it all together
/// </summary>
public class SceneDescriptor
{
    private static readonly ILogger<SceneDescriptor> Log = Logging.GetLogger<SceneDescriptor>();

    /// <summary>
    /// non scene related render options
    /// </summary>
    public RenderOptions Options { get; set; } 
    
    /// <summary>
    /// reusable map of texture definitions, keyed by string to make referencing them easier
    /// </summary>
    public Dictionary<string, ITextureGenerator> Textures { get; init; } = new();

    /// <summary>
    /// reusable map of materials, same as textures
    /// </summary>
    public Dictionary<string, IMaterialGenerator> Materials { get; init; } = new();

    /// <summary>
    /// scene objects
    /// </summary>
    public List<IPrimitiveGenerator> Primitives { get; init; } = [];

    public RGBColor Background { get; init; } = new(0.7f, 0.8f, 1f);
    
    // public List<IPrimitiveGenerator> Lights { get; init; } = [];
    
    public CameraDescriptor Camera { get; init; }

    public Scene Build(ParserContext context)
    {
        // step 1, convert all textures to their real representation
        var textures =
            Textures.ToDictionary(
                ks => ks.Key,
                vs => vs.Value.Generate(context));

        // step 2, do the same for materials and use texture references from above
        var materials = Materials.ToDictionary(
            ks => ks.Key,
            vs => vs.Value.Generate(textures));

        var primList = Primitives.Aggregate(new List<IPrimitive>(), (acc, prim) =>
            {
                acc.AddRange(prim.Generate(materials, context));
                return acc;
            });

        // var lightSources = Lights.Aggregate(new List<IPrimitive>(), (acc, prim) =>
        // {
        //     acc.AddRange(prim.Generate(materials, context));
        //     return acc;
        // });
        
        // light sources also need to show up as regular primitives
        // primList.AddRange(lightSources);

        if (Options == null)
        {
            Log.LogWarning("no options defined, use defaults");
            Options = new RenderOptions();
        }
        
        return new Scene
        {
            World = new BVHTree(primList),
            Background = Background,
            Options = Options,
            // Lights = new PrimitiveList(lightSources), // crashes with BVH and single item
            Camera = new PerspectiveCamera(Camera, Options)
        };
    }
}