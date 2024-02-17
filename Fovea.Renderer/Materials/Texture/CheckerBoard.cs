using Fovea.Renderer.Core;
using static System.MathF;

namespace Fovea.Renderer.Materials.Texture;

public class CheckerBoard(ITexture even, ITexture odd, int size = 10) : ITexture
{
    public RGBColor Value(float u, float v, Vector3 p)
    {
        var sines = Sin(size * p.X) * Sin(size * p.Y) * Sin(size * p.Z);
        return sines < 0 ? odd.Value(u, v, p) : even.Value(u, v, p);
    }
}