using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials
{
    public class DiffuseLight : IMaterial
    {
        private readonly ITexture _color;

        public DiffuseLight(ITexture color)
        {
            _color = color;
        }
        
        public bool Scatter(in Ray rayIn, HitRecord hitRecord, ref ScatterResult scatterResult)
        {
            return false;
        }

        public RGBColor Emitted(double u, double v, Point3 p) => _color.Value(u, v, p);
    }
}