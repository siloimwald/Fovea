using System.Collections.Generic;
using System.Linq;

namespace Fovea.Renderer.VectorMath;

/// <summary>
/// thin wrapper around a list of 4x4 matrices
/// </summary>
public class Transform
{
    private readonly List<Matrix4x4> _transformations = new List<Matrix4x4>();

    public Transform WithTranslation(float tx, float ty, float tz)
    {
        _transformations.Add(Matrix4x4.CreateTranslation(tx, ty, tz));
        return this;
    }

    public Transform WithTranslation(Vector3 translationVector)
    {
        return WithTranslation(translationVector.X, translationVector.Y, translationVector.Z);
    }

    public Transform WithScaling(float sx, float sy, float sz)
    {
        _transformations.Add(Matrix4x4.CreateScale(sx, sy, sz));
        return this;
    }

    public Transform WithScaling(Vector3 scalingVector)
    {
        return WithScaling(scalingVector.X, scalingVector.Y, scalingVector.Z);
    }

    public Transform WithRotation(Axis axis, float degree)
    {
        var radian = MathUtils.DegToRad(degree);
        var m = axis switch
        {
            Axis.X => Matrix4x4.CreateRotationX(radian),
            Axis.Y => Matrix4x4.CreateRotationY(radian),
            _ => Matrix4x4.CreateRotationZ(radian)
        };
        _transformations.Add(m);
        return this;
    }

    public (Matrix4x4 forward, Matrix4x4 inverse, bool invertFailed) Build()
    {
        var totalMatrix = _transformations.Aggregate(Matrix4x4.Identity, (acc, tMatrix) => acc * tMatrix);
        var couldInvert = Matrix4x4.Invert(totalMatrix, out var inverse);
        return (totalMatrix, inverse, couldInvert);
    }

    public Matrix4x4 BuildForwardOnly()
    {
        return _transformations.Aggregate(Matrix4x4.Identity, (acc, tMatrix) => acc * tMatrix);
    }
}