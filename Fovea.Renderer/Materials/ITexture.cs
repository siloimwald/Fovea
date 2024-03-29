using Fovea.Renderer.Core;

namespace Fovea.Renderer.Materials;

public interface ITexture
{
    RGBColor Value(float u, float v, Vector3 p);
}