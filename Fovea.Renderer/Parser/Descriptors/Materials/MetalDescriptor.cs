using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

public class MetalDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public float Fuzzy { get; init; } = 1.0f;

    public IMaterial Generate(IDictionary<string, ITexture> textures)
        => new Metal(GetTextureOrFail(textures), Fuzzy);
}