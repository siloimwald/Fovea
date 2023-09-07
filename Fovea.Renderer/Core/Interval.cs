using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Fovea.Renderer.Core;

public struct Interval
{
    /// <summary>
    /// the left bound/min value of this interval
    /// </summary>
    public float Min;
    
    /// <summary>
    /// the right bound/max value of this interval
    /// </summary>
    public float Max;
    
    /// <summary>
    /// creates an empty interval [+oo, -oo]
    /// </summary>
    public Interval()
    {
        Min = float.PositiveInfinity;
        Max = float.NegativeInfinity;
    }

    /// <summary>
    /// creates the interval [min, max]
    /// </summary>
    /// <param name="min">left/min bound of interval</param>
    /// <param name="max">right/max bound of interval</param>
    public Interval(float min, float max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// tests if this interval contains the value t, i.e. t \in [min, max]
    /// </summary>
    /// <param name="t">float test value</param>
    /// <returns>true if t is contained within the interval</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Contains(float t)
    {
        return Min <= t && t <= Max;
    }

    /// <summary>
    /// tests if this interval contains t without t being exactly on the interval
    /// i.e. t \in ]min, max[
    /// boundaries
    /// </summary>
    /// <param name="t">float test value</param>
    /// <returns>true if t is within the interval and not exactly on either boundary</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Surrounds(float t)
    {
        return Min < t && t < Max;
    }

    /// <summary>
    /// creates a often used interval which offsets the interval a little bit from
    /// zero to avoid self intersection and is bounded by +oo
    /// </summary>
    /// <returns>the interval [e, +oo]</returns>
    public static Interval HalfOpenWithOffset()
    {
        return new Interval(1e-4f, float.PositiveInfinity);
    }

    /// <summary>
    /// the interval [-oo, +oo]
    /// </summary>
    /// <returns></returns>
    public static Interval Everything()
    {
        return new Interval(float.NegativeInfinity, float.PositiveInfinity);
    }
    
}