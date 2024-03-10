using System;
using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Parser.Descriptors.Materials;
using Fovea.Renderer.Parser.Descriptors.Primitives;
using Fovea.Renderer.Parser.Descriptors.Textures;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.Primitives;
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
    public required Dictionary<string, ITextureGenerator> Textures { get; init; } = new();

    /// <summary>
    /// reusable map of materials, same as textures
    /// </summary>
    public required Dictionary<string, IMaterialGenerator> Materials { get; init; } = new();

    /// <summary>
    /// scene objects
    /// </summary>
    public List<IPrimitiveGenerator> Primitives { get; init; } = [];

    /// <summary>
    /// references scene primitives by id, which should be added (again) to the
    /// importance list of the scene, i.e. for directly sampling light sources
    /// </summary>
    public List<string> ImportanceList { get; init; } = [];
    
    public RGBColor Background { get; init; } = new(0.7f, 0.8f, 1f);

    /// <summary>
    /// instance-able objects, instances provide transformation and different material
    /// </summary>
    public Dictionary<string, IPrimitiveGenerator> Blueprints { get; init; } = new();
    
    public CameraDescriptor Camera { get; init; }

    public Scene Build(ParserContext context)
    {
        // keep track of stuff we should dispose after rendering, mostly image textures for now
        var disposables = new List<IDisposable>();
        
        // step 1, convert all textures to their real representation
        var textures =
            Textures.ToDictionary(
                ks => ks.Key,
                vs =>
                {
                    var texture = vs.Value.Generate(context);
                    if (texture is IDisposable disposable)       
                        disposables.Add(disposable);
                    return texture;
                });

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
            context.Blueprints[key] = val.Generate(context);
        }

        var importanceList = new PrimitiveList();
        var primitiveList = new List<IPrimitive>();
        foreach (var primGenerator in Primitives)
        {
            var generatedPrimitive = primGenerator.Generate(context);
            primitiveList.Add(generatedPrimitive);
            if (string.IsNullOrEmpty(primGenerator.Id)) continue;
            if (ImportanceList.Contains(primGenerator.Id))
            {
                importanceList.Add(generatedPrimitive);
                Log.LogInformation("adding {Id} to importance list", primGenerator.Id);
            }
            else
            {
                Log.LogWarning("reference id {Id} for importance list not found or unused", primGenerator.Id);
            }
        }

        if (ImportanceList.Count != importanceList.Count)
        {
            Log.LogWarning("importance list mismatch");
        }
        
        if (Options == null)
        {
            Log.LogWarning("no options defined, use defaults");
            Options = new RenderOptions();
        }
        
        return new Scene(disposables)
        {
            World = new BVHTree(primitiveList),
            Background = Background,
            Options = Options,
            ImportanceSamplingList = importanceList,
            Camera = new PerspectiveCamera(Camera, Options)
        };
    }
}