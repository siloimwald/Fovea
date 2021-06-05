using Fovea.Renderer.Image;
using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core
{
    public class Scene
    {
        public IPrimitive World { get; set; }
        public IPrimitive Lights { get; set; }
        public PerspectiveCamera Camera { get; set; }
        public RGBColor Background { get; set; } = new(0.7, 0.8, 1);

        public (int imageWidth, int ImageHeight) OutputSize { get; set; }

        // hacky attempt at skybox
        public IPrimitive Environment { get; set; }
    }
}