using Fovea.Renderer.Image;

namespace Fovea.Renderer.Core
{
    /// <summary>
    /// result of material interaction, we'll see how good this one plays out
    /// </summary>
    public class ScatterResult
    {
        public RGBColor Attenuation { get; set; }
        public Ray OutgoingRay { get; set; }
    }
}