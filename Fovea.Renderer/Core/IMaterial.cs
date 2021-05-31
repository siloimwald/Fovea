using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public interface IMaterial
    {
        /// <summary>
        /// determine result for material interaction, if any
        /// </summary>
        /// <param name="rayIn">incoming ray</param>
        /// <param name="hitRecord">record with all the important intersection bits</param>
        /// <param name="scatterResult">result of interaction</param>
        /// <returns>true if ScatterResult contents should be used (and are set)</returns>
        bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult);

        /// <summary>
        /// emitted color for this material, if any. defaults to black
        /// </summary>
        /// <param name="u">texture coordinate u</param>
        /// <param name="v">texture coordinate v</param>
        /// <param name="p">hit point on surface</param>
        /// <returns></returns>
        RGBColor Emitted(double u, double v, Point3 p)
        {
            return new(); // black
        }

        double ScatterPDF(in Ray ray, in HitRecord hitRecord, in Ray scatteredRay)
        {
            return 0;
        }
    }
}