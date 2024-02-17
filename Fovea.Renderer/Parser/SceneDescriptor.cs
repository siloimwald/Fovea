using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Parser.Yaml;

namespace Fovea.Renderer.Parser;

/// <summary>
/// scene descriptor is the parent parser node for our yaml scene file
/// and sticks it all together
/// </summary>
public class SceneDescriptor
{
    public RenderOptions Options { get; init; }

    /// <summary>
    /// reusable map of texture definitions, keyed by string to make referencing them easier
    /// </summary>
    public Dictionary<string, ITextureGenerator> Textures { get; init; }

    /// <summary>
    /// reusable map of materials, same as textures
    /// </summary>
    public Dictionary<string, IMaterialGenerator> Materials { get; init; }

    /// <summary>
    /// scene objects
    /// </summary>
    public List<IPrimitiveGenerator> Primitives { get; init; }

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
        
        return new Scene
        {
            World = new BVHTree(primList),
            Background = new RGBColor(0.2f, 0.2f, 0.2f),
            Options = Options,
            Camera = Camera.AsPerspectiveCamera(Options.ImageWidth/(float)Options.ImageHeight)
        };
    }
}