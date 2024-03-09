using Fovea.Renderer.Primitives;
using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core;

public class Scene
{
    /// <summary>
    /// scene objects
    /// </summary>
    public IPrimitive World { get; init; }
    
    /// <summary>
    /// in the books, this is called light source(s), but, as the last example scene with the
    /// glass sphere demonstrates, basically is a list of things that should be sampled
    /// explicitly. And i hopefully understood that correctly :)
    /// </summary>
    public PrimitiveList ImportanceSamplingList { get; set; }
    
    public PerspectiveCamera Camera { get; set; }
    
    public RGBColor Background { get; init; } = new(0.7f, 0.8f, 1f);

    public RenderOptions Options { get; init; }
}