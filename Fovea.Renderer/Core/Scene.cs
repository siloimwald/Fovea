using Fovea.Renderer.Image;
using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core;

public class Scene
{
    public IPrimitive World { get; set; }
    public IPrimitive Lights { get; set; }
    public PerspectiveCamera Camera { get; set; }
    public RGBColor Background { get; set; } = new(0.7f, 0.8f, 1f);

    public (int imageWidth, int ImageHeight) OutputSize { get; set; }

    // hacky attempt at skybox
    public IPrimitive Environment { get; set; }
}