using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

// a collection of parser entities/descriptors that map to materials

public class DiffuseLightDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public IMaterial Generate(IDictionary<string, ITexture> textures) => new DiffuseLight(GetTextureOrFail(textures));
}
