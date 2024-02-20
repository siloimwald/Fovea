using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;

namespace Fovea.Renderer.Mesh;

/// <summary>static factory for a box made of triangles</summary>
public static class BoxProducer
{
    public static PrimitiveList MakeBox(Vector3 pointA, Vector3 pointB, IMaterial material)
    {
        var sides = new PrimitiveList();
        var min = Vector3.Min(pointA, pointB);
        var max = Vector3.Max(pointA, pointB);
        var dx = new Vector3(max.X - min.X, 0, 0);
        var dy = new Vector3(0, max.Y - min.Y, 0);
        var dz = new Vector3(0, 0, max.Z - min.Z);
        sides.AddRange([
            new Quad(new Vector3(min.X, min.Y, max.Z), dx, dy, material), // front
            new Quad(new Vector3(max.X, min.Y, max.Z), -dz, dy, material), // right
            new Quad(new Vector3(max.X, min.Y, min.Z), -dx, dy, material), // back
            new Quad(new Vector3(min.X, min.Y, min.Z), dz, dy, material), // left
            new Quad(new Vector3(min.X, max.Y, max.Z), dx, -dz, material), // top
            new Quad(new Vector3(min.X, min.Y, min.Z), dx, dz, material), // bottom
        ]);
        return sides;
    }
}