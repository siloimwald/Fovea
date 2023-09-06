
namespace Fovea.Renderer.Sampling
{
    public interface IPDF
    {
        /// <summary>evaluate the pdf for the given direction</summary>
        /// <param name="direction">sampled direction</param>
        /// <returns></returns>
        float Evaluate(Vector3 direction);

        /// <summary>generate a direction according to the underlying pdf</summary>
        /// <returns></returns>
        Vector3 Generate();
    }
}