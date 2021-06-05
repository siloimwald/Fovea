using Fovea.Renderer.Image;

namespace Fovea.Renderer.Core
{
    public interface IMaterial
    {
        /// <summary>determine result for material interaction, if any</summary>
        /// <param name="rayIn">incoming ray</param>
        /// <param name="hitRecord">record with all the important intersection bits</param>
        /// <param name="scatterResult">result of interaction</param>
        /// <returns>true if ScatterResult contents should be used (and are set)</returns>
        bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult);

        /// <summary>emitted color for this material, if any. defaults to black</summary>
        /// <param name="ray">incoming ray</param>
        /// <param name="hitRecord">intersection parameters</param>
        /// <returns></returns>
        RGBColor Emitted(in Ray ray, in HitRecord hitRecord)
        {
            return new(); // black
        }

        double ScatteringPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
        {
            return 0;
        }
    }
}