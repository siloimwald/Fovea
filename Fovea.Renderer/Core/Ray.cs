namespace Fovea.Renderer.Core;

/// <summary>
///     a ray in 3d space starting at some origin and pointing at some direction. This is way too big for a struct by
///     any guidelines, but is allocated all over the place many many times
/// </summary>
public readonly struct Ray
{
    public readonly Vector3 Origin;
    public readonly Vector3 Direction;
    public readonly float Time;
    public readonly Vector3 InverseDirection;

    public Ray(Vector3 origin, Vector3 direction, float time = 0.0f)
    {
        Origin = origin;
        Direction = direction;
        Time = time;
        InverseDirection = new Vector3(1.0f / Direction.X, 1.0f / Direction.Y, 1.0f / Direction.Z);
    }

    /// <summary>returns the point where this is ray is pointing to for a given ray parameter t</summary>
    /// <param name="t">ray parameter</param>
    /// <returns></returns>
    public Vector3 PointsAt(float t)
    {
        return Origin + Direction * t;
    }
}