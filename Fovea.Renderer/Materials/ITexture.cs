using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials
{
    public interface ITexture
    {
        RGBColor Value(double u, double v, Point3 p);
    }
}