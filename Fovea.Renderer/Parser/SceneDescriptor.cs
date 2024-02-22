using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

    /// <summary>
    /// instance-able objects, instances provide transformation and different material
    /// </summary>
    public Dictionary<string, IPrimitiveGenerator> Blueprints { get; init; } = new();
    
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

        // pass materials and texture into context
        // note that blueprints might use these as well
        context.Materials = materials;
        context.Textures = textures;
        
        // slight hack so blueprints/instancing fits into the existing interface
        // needs more work to actually instance whole meshes (idea: whole mesh is one bvh on its own...)
        context.Blueprints = new Dictionary<string, IPrimitive>();
        foreach (var (key, val) in Blueprints)
        {
            var prims = val.Generate(context);
            if (prims.Count > 1)
            {
                Log.LogWarning("blueprints generating more than one primitive not supported yet");
            }

            context.Blueprints[key] = prims[0];
        }
        
 
        
        // generate all the primitives
        var primList = Primitives.Aggregate(new List<IPrimitive>(), (acc, prim) =>
            {
                acc.AddRange(prim.Generate(context));
                return acc;
            });

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