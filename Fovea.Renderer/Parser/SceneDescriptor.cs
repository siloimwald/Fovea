using System.Collections.Generic;

namespace Fovea.Renderer.Parser;

/// <summary>
/// scene descriptor is the parent parser node for our yaml scene file
/// and sticks it all together
/// </summary>
public class SceneDescriptor
{
    public RenderOptionsDescriptor Options { get; init; }
    
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
}