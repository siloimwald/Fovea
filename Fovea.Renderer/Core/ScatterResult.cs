using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;

namespace Fovea.Renderer.Core
{
    /// <summary>
    /// result of material interaction, we'll see how good this one plays out
    /// </summary>
    public struct ScatterResult
    {
        public RGBColor Attenuation;
        public bool IsSpecular;
        public Ray SpecularRay;
        public IPDF Pdf;
    }
}