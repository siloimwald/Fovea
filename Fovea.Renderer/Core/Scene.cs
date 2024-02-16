using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core;

public class Scene
{
    public IPrimitive World { get; init; }
    public IPrimitive Lights { get; set; }
    public PerspectiveCamera Camera { get; set; }
    public RGBColor Background { get; init; } = new(0.7f, 0.8f, 1f);

    public RenderOptions Options { get; init; }

    
    // hacky attempt at skybox
    public IPrimitive Environment { get; set; }
}