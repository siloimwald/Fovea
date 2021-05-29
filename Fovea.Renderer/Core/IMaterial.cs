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

        RGBColor Emitted(double u, double v, Point3 p)
        {
            return new RGBColor();
        }
    }
}