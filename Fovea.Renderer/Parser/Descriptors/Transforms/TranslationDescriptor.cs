namespace Fovea.Renderer.Parser.Descriptors.Transforms;

public class TranslationDescriptor : ITransformationDescriptor
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }
    public Matrix4x4 GetTransformationMatrix() => Matrix4x4.CreateTranslation(X, Y, Z);
}