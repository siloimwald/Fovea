using System.Text.Json.Serialization;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Parser.Descriptors.Transforms;

public class RotationDescriptor : ITransformationDescriptor
{
    [JsonPropertyName("by")] // degree
    public float Angle { get; init; } = 0f;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Axis Axis { get; init; } = Axis.X;

    /// <summary>
    /// rotation center
    /// </summary>
    public Vector3 Center { get; init; } = Vector3.Zero;

    public Matrix4x4 GetTransformationMatrix()
    {
        var rad = MathUtils.DegToRad(Angle);
        // add center of rotation later
        return Axis switch
        {
            Axis.X => Matrix4x4.CreateRotationX(rad, Center),
            Axis.Y => Matrix4x4.CreateRotationY(rad, Center),
            _ => Matrix4x4.CreateRotationZ(rad, Center)
        };
    }
}