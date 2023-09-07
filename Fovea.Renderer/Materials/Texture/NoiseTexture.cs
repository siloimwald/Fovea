using System;
using Fovea.Renderer.Image;

namespace Fovea.Renderer.Materials.Texture
{
    public class NoiseTexture : ITexture
    {
        private readonly Perlin _perlin;
        private readonly float _scale;

        public NoiseTexture(float scale)
        {
            _scale = scale;
            _perlin = new Perlin();
        }

        public RGBColor Value(float u, float v, Vector3 p)
        {
            var noise = 1 + MathF.Sin(_scale * p.Z + _perlin.Turbulence(p) * 10);
            return new RGBColor(1) * (float) (0.5 * noise);
        }
    }
}