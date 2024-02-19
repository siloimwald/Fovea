using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.VectorMath;

/// <summary>
///     axis aligned bounding box defined by min/max points along the principal axes. Taken from one of my previous
///     renderer implementations
/// </summary>
public class BoundingBox
{
    private const byte ShuffleMask = (3 << 4) | (2 << 2) | 1;

    /// <summary>extent of this bounding box on the X axis</summary>
    public readonly Interval ExtentX;

    /// <summary>extent of this bounding box on the Y axis</summary>
    public readonly Interval ExtentY;

    /// <summary>extent of this bounding box on the Z axis</summary>
    public readonly Interval ExtentZ;

    /// <summary>create a new bounding box with the given min/max. Those are assumed to be correctly ordered already.</summary>
    /// <param name="min">min. points along all dimensions</param>
    /// <param name="max">max. points along all dimensions</param>
    public BoundingBox(Vector3 min, Vector3 max)
        : this(new Interval(MathF.Min(min.X, max.X), MathF.Max(min.X, max.X)),
            new Interval(MathF.Min(min.Y, max.Y), MathF.Max(min.Y, max.Y)),
            new Interval(MathF.Min(min.Z, max.Z), MathF.Max(min.Z, max.Z)))
    {
    }

    private BoundingBox(Interval x, Interval y, Interval z)
    {
        ExtentX = x;
        ExtentY = y;
        ExtentZ = z;

        const float delta = 0.0001f;
        if (ExtentX.Size < delta)
        {
            ExtentX = ExtentX.Expand(delta);
        }

        if (ExtentY.Size < delta)
        {
            ExtentY = ExtentY.Expand(delta);
        }

        if (ExtentZ.Size < delta)
        {
            ExtentZ = ExtentZ.Expand(delta);
        }
    }

    /// <summary>unite two bounding boxes by computing the minimal box that fully contains both input parameters</summary>
    /// <param name="boxA">bounding box object</param>
    /// <param name="boxB">bounding box object</param>
    /// <returns>Box = boxA U boxB </returns>
    public static BoundingBox Union(BoundingBox boxA, BoundingBox boxB)
    {
        var x = new Interval(boxA.ExtentX, boxB.ExtentX);
        var y = new Interval(boxA.ExtentY, boxB.ExtentY);
        var z = new Interval(boxA.ExtentZ, boxB.ExtentZ);
        return new BoundingBox(x, y, z);
    }

    /// <summary>box center</summary>
    public Vector3 GetCentroid()
    {
        return new Vector3(ExtentX.Min, ExtentY.Min, ExtentZ.Min) + GetExtent() * 0.5f;
    }

    public Vector3 GetExtent()
    {
        return new Vector3(ExtentX.Size, ExtentY.Size, ExtentZ.Size);
    }

    /// <summary>compute the volume of bounding box</summary>
    /// <returns></returns>
    public float GetVolume()
    {
        var ext = GetExtent();
        return ext.X * ext.Y * ext.Z;
    }

    /// <summary>area of bounding box</summary>
    /// <returns>area of this bounding box</returns>
    public float GetArea()
    {
        var ext = GetExtent();
        return 2.0f * (ext.X * ext.Y + ext.Y * ext.Z + ext.Z * ext.X);
    }

    /// <summary>test whether the given ray intersects this bounding box</summary>
    /// <param name="ray">ray to test against</param>
    /// <param name="interval">current ray interval</param>
    /// <returns>true if ray intersects box</returns>
    public bool Intersect(in Ray ray, Interval interval)
    {
        for (var a = 0; a < 3; a++)
        {
            var t0 = (Axis(a).Min - ray.Origin[a]) * ray.InverseDirection[a];
            var t1 = (Axis(a).Max - ray.Origin[a]) * ray.InverseDirection[a];

            if (ray.InverseDirection[a] < 0)
            {
                (t0, t1) = (t1, t0);
            }

            if (t0 > interval.Min)
            {
                interval.Min = t0;
            }

            if (t1 < interval.Max)
            {
                interval.Max = t1;
            }
   
            if (interval.Max <= interval.Min)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// get the interval along the requested axis, just for compatibility with the book
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Interval Axis(int axis)
    {
        return axis switch
        {
            0 => ExtentX,
            1 => ExtentY,
            _ => ExtentZ
        };
    }

    // public bool IntersectAvx(in Ray ray, in Interval interval)
    // {
    //     var invDir = Vector256.Create(ray.InverseDirection.AsVector128(),
    //         ray.InverseDirection.AsVector128());
    //     var org = Vector256.Create(ray.Origin.AsVector128(), ray.Origin.AsVector128());
    //     var minMax = Vector256.Create(ExtentX.Min, ExtentY.Min, ExtentZ.Min, 0.0f,
    //         ExtentX.Max, ExtentY.Max, ExtentZ.Max, 0.0f);
    //
    //     var t = Avx.Multiply(Avx.Subtract(minMax, org), invDir);
    //     
    //     var t0 = Avx.ExtractVector128(t, 0);
    //     var t1 = Avx.ExtractVector128(t, 128);
    //     var min = Sse.Min(t0, t1);
    //     var max = Sse.Max(t0, t1);
    //    
    //     // compares min0 and min1 and min1 and min2
    //     // shuffle again and compare to get overall min/max in first component
    //     var minStage0 = Sse.Max(Sse.Shuffle(min, min, ShuffleMask), min);
    //     var gTMin = Sse.Max(Sse.Shuffle(minStage0, minStage0, ShuffleMask), minStage0).GetElement(0);
    //     var maxStage0 = Sse.Min(Sse.Shuffle(max, max, ShuffleMask), max);
    //     var gTMax = Sse.Min(Sse.Shuffle(maxStage0, maxStage0, ShuffleMask), maxStage0).GetElement(0);
    //
    //     gTMin = MathF.Max(interval.Min, gTMin);
    //     gTMax = MathF.Min(interval.Max, gTMax);
    //     return gTMax >= gTMin && gTMax > 0;
    // }
    
    public bool IntersectSse(in Ray ray, in Interval interval)
    {
        var invDir = ray.InverseDirection.AsVector128();
        var org = ray.Origin.AsVector128();
        var minVec = Vector128.Create(ExtentX.Min, ExtentY.Min, ExtentZ.Min, 0.0f);
        var maxVec = Vector128.Create(ExtentX.Max, ExtentY.Max, ExtentZ.Max, 0.0f);

        var t0 = Sse.Multiply(Sse.Subtract(minVec, org), invDir);
        var t1 = Sse.Multiply(Sse.Subtract(maxVec, org), invDir);
        
        var min = Sse.Min(t0, t1);
        var max = Sse.Max(t0, t1);
       
        // compares min0 and min1 and min1 and min2
        // shuffle again and compare to get overall min/max in first component
        var minStage0 = Sse.Max(Sse.Shuffle(min, min, ShuffleMask), min);
        var gTMin = Sse.Max(Sse.Shuffle(minStage0, minStage0, ShuffleMask), minStage0).GetElement(0);
        var maxStage0 = Sse.Min(Sse.Shuffle(max, max, ShuffleMask), max);
        var gTMax = Sse.Min(Sse.Shuffle(maxStage0, maxStage0, ShuffleMask), maxStage0).GetElement(0);

        gTMin = MathF.Max(interval.Min, gTMin);
        gTMax = MathF.Min(interval.Max, gTMax);
        return gTMax >= gTMin && gTMax > 0;
    }

    /// <summary>
    ///     creates a bounding box with bounds [maxFloat,maxFloat,maxFloat] to [-maxFloat, -maxFloat, -maxFloat] to be
    ///     used for union loops
    /// </summary>
    /// <returns>inverted, maximal empty bounding box :)</returns>
    public static BoundingBox CreateMaxEmptyBox() => new(Interval.Empty, Interval.Empty, Interval.Empty);

    /// <summary>
    ///     from pbrt book. used for bin projection, given p as a primitive centroid and this as the centroid bounds of
    ///     all primitives it scales offsets of primitive centroids from 0,0,0 to 1,1,1
    /// </summary>
    /// <param name="p">centroid of primitive box</param>
    /// <returns></returns>
    public Vector3 Offset(Vector3 p)
    {
        var o = new Vector3(p.X - ExtentX.Min, p.Y - ExtentY.Min, p.Z - ExtentZ.Min);
        var ext = GetExtent();

        // avoid division by zero
        return new Vector3(
            ext.X > 0 ? o.X / ext.X : o.X,
            ext.Y > 0 ? o.Y / ext.Y : o.Y,
            ext.Z > 0 ? o.Z / ext.Z : o.Z);
    }

    public BoundingBox Transform(Matrix4x4 transform)
    {
        throw new NotImplementedException();
        // var transformedMin = Vector3.Transform(_min, transform);
        // var transformedMax = Vector3.Transform(_max, transform);
        // return new BoundingBox(
        //     Vector3.Min(transformedMin, transformedMax),
        //     Vector3.Max(transformedMin, transformedMax));
    }

    public override string ToString()
    {
        return $"Box[{ExtentX}, {ExtentY}, {ExtentZ}]";
    }
}