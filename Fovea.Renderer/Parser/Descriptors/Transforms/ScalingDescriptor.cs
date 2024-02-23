namespace Fovea.Renderer.Parser.Descriptors.Transforms;

public class ScalingDescriptor : ITransformationDescriptor
{
    public float X { get; init; } = 1;
    public float Y { get; init; } = 1;
    public float Z { get; init; } = 1;
    public Matrix4x4 GetTransformationMatrix() => Matrix4x4.CreateScale(X, Y, Z);
}