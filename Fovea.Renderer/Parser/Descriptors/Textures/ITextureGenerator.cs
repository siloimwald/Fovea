using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Textures;

[JsonDerivedType(typeof(NoiseTextureDescriptor), typeDiscriminator:"noise")]
[JsonDerivedType(typeof(ImageTextureDescriptor), typeDiscriminator:"image")]
[JsonDerivedType(typeof(RGBColor), typeDiscriminator:"color")]
public interface ITextureGenerator
{
    ITexture Generate(ParserContext context);
}