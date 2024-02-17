using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives;

public class PrimitiveList(List<IPrimitive> prims) : IPrimitive
{
    public PrimitiveList() : this(new List<IPrimitive>())
    {
    }

    public IPrimitive this[int index] => prims[index];

    public bool Hit(in Ray ray, in Interval rayInterval, ref HitRecord hitRecord)
    {
        var hitSomething = false;
        hitRecord.RayT = rayInterval.Max;
        for (var p = 0; p < prims.Count; ++p)
            if (prims[p].Hit(ray, rayInterval with { Max = hitRecord.RayT }, ref hitRecord))
                hitSomething = true;

        return hitSomething;
    }


    public BoundingBox GetBoundingBox(float t0, float t1)
    {
        return
            prims
                .Select(p => p.GetBoundingBox(t0, t1))
                .Aggregate(BoundingBox.CreateMaxEmptyBox(), BoundingBox.Union);
    }

    public float PdfValue(Vector3 origin, Vector3 direction)
    {
        var weight = 1.0f / prims.Count;
        return prims.Sum(p => weight * p.PdfValue(origin, direction));
    }

    public Vector3 RandomDirection(Vector3 origin)
    {
        var pIndex = Sampler.Instance.RandomInt(0, prims.Count);
        return prims[pIndex].RandomDirection(origin);
    }

    public void Add(IPrimitive p)
    {
        prims.Add(p);
    }
}