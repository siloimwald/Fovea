using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;

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

    public Scene Build()
    {
        // step 1, convert all textures to their real representation
        var textures =
            Textures.ToDictionary(
                ks => ks.Key,
                vs => vs.Value.Generate());

        // step 2, do the same for materials and use texture references from above
        var materials = Materials.ToDictionary(
            ks => ks.Key,
            vs => vs.Value.Generate(textures));

        var list = new List<IPrimitive>();

        foreach (var primDescriptor in Primitives)
        {
            primDescriptor.Generate(materials, list);
        }
        
        return new Scene
        {
            World = new BVHTree(list),
            Background = new RGBColor(0.2f, 0.2f, 0.2f),
            Options = Options,
            Camera = Camera.AsPerspectiveCamera(Options.ImageWidth/(float)Options.ImageHeight)
        };
    }
}