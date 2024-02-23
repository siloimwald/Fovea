using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

public class MatteDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public IMaterial Generate(IDictionary<string, ITexture> textures)
        => new Matte(GetTextureOrFail(textures));
}