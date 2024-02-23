using System.Text.Json.Serialization;

namespace Fovea.Renderer.Parser.Descriptors.Transforms;

[JsonDerivedType(typeof(TranslationDescriptor), "translate")]
[JsonDerivedType(typeof(ScalingDescriptor), "scale")]
[JsonDerivedType(typeof(RotationDescriptor), "rotate")]
public interface ITransformationDescriptor
{
    Matrix4x4 GetTransformationMatrix();
}