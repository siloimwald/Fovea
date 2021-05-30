using System;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials.Texture
{
    public class NoiseTexture : ITexture
    {
        private readonly Perlin _perlin;
        private readonly double _scale;

        public NoiseTexture(double scale)
        {
            _scale = scale;
            _perlin = new Perlin();
        }

        public RGBColor Value(double u, double v, Point3 p)
        {
            var noise = 1 + Math.Sin(_scale * p.PZ + _perlin.Turbulence(p) * 10);
            return new RGBColor(1) * (float) (0.5 * noise);
        }
    }
}