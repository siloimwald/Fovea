using Fovea.Renderer.Image;

namespace Fovea.Renderer.Core
{
    /// <summary>
    /// result of material interaction, we'll see how good this one plays out
    /// </summary>
    public struct ScatterResult
    {
        public RGBColor Attenuation;
        public Ray OutgoingRay;
        public double PdfValue;
    }
}