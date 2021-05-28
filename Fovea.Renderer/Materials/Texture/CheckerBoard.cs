using System;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials.Texture
{
    public class CheckerBoard : ITexture
    {
        private readonly ITexture _even;
        private readonly ITexture _odd;
        private readonly int _size;

        public CheckerBoard(ITexture even, ITexture odd, int size = 10)
        {
            _even = even;
            _odd = odd;
            _size = size;
        }
        
        public RGBColor Value(double u, double v, Point3 p)
        {
            var sines = Math.Sin(_size * p.PX) * Math.Sin(_size * p.PY) * Math.Sin(_size * p.PZ);
            return sines < 0 ? _odd.Value(u, v, p) : _even.Value(u, v, p);
        }
    }
}