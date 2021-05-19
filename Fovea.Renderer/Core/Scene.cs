using System.Collections.Generic;
using Fovea.Renderer.Image;
using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core
{
    public class Scene
    {
        public IPrimitive World { get; set; }
        public PerspectiveCamera Camera { get; set; }
        public ImageSize OutputSize { get; set; }
    }

}