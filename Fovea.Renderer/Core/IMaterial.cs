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
        bool Scatter(Ray rayIn, HitRecord hitRecord, ScatterResult scatterResult);
    }
}