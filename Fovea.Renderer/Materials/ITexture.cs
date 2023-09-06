using Fovea.Renderer.Image;

namespace Fovea.Renderer.Materials
{
    public interface ITexture
    {
        RGBColor Value(double u, double v, Vector3 p);
    }
}