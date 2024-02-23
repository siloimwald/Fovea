using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;

namespace Fovea.Renderer.Parser.Descriptors.Materials;

public class DielectricDescriptor : IMaterialGenerator
{
    [JsonPropertyName("ior")]
    public required float IOR { get; init; }

    public IMaterial Generate(IDictionary<string, ITexture> textures) => new Dielectric(IOR);
}