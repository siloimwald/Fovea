using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Sampling
{
    public interface IPDF
    {
        /// <summary>evaluate the pdf for the given direction</summary>
        /// <param name="direction">sampled direction</param>
        /// <returns></returns>
        double Evaluate(Vec3 direction);

        /// <summary>generate a direction according to the underlying pdf</summary>
        /// <returns></returns>
        Vec3 Generate();
    }
}