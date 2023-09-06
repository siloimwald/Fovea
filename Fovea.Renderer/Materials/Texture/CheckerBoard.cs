using Fovea.Renderer.Image;
using static System.MathF;

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

        public RGBColor Value(double u, double v, Vector3 p)
        {
            var sines = Sin(_size * p.X) * Sin(_size * p.Y) * Sin(_size * p.Z);
            return sines < 0 ? _odd.Value(u, v, p) : _even.Value(u, v, p);
        }
    }
}