using System;

namespace Fovea.Renderer.VectorMath;

public readonly struct OrthonormalBasis
{
    public readonly Vector3 UAxis;
    public readonly Vector3 VAxis;
    public readonly Vector3 WAxis;

    public OrthonormalBasis(Vector3 w)
    {
        WAxis = Vector3.Normalize(w);
        var a = MathF.Abs(WAxis.X) > 0.9f ? Vector3.UnitY : Vector3.UnitX;
        VAxis = Vector3.Normalize(Vector3.Cross(WAxis, a));
        UAxis = Vector3.Cross(WAxis, VAxis);
    }

    public Vector3 Local(float a, float b, float c)
    {
        var v = UAxis * a + VAxis * b + WAxis * c;
        return new Vector3(v.X, v.Y, v.Z);
    }

    public Vector3 Local(Vector3 v) => Local(v.X, v.Y, v.Z);
}