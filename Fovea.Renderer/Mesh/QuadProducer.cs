using System.Collections.Generic;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Mesh;

public static class QuadProducer
{
    /// <summary>create mesh for a quad with sd0-ed0, sd1-ed1 at the given d3 position on some axis</summary>
    /// <returns></returns>
    public static TriangleMesh Produce(
        float minDim1,
        float maxDim1,
        float minDim2,
        float maxDim2,
        float d3,
        Axis axis)
    {
        (minDim1, maxDim1) = minDim1 > maxDim1 ? (maxDim1, minDim1) : (minDim1, maxDim1);
        (minDim2, maxDim2) = minDim2 > maxDim2 ? (maxDim2, minDim2) : (minDim2, maxDim2);

        var vertices = new List<Vector3>();

        if (axis == Axis.X)
        {
            vertices.Add(new Vector3(d3, minDim1, minDim2));
            vertices.Add(new Vector3(d3, maxDim1, minDim2));
            vertices.Add(new Vector3(d3, maxDim1, maxDim2));
            vertices.Add(new Vector3(d3, minDim1, maxDim2));
        }
        else if (axis == Axis.Y)
        {
            vertices.Add(new Vector3(minDim1, d3, minDim2));
            vertices.Add(new Vector3(maxDim1, d3, minDim2));
            vertices.Add(new Vector3(maxDim1, d3, maxDim2));
            vertices.Add(new Vector3(minDim1, d3, maxDim2));
        }
        else
        {
            vertices.Add(new Vector3(minDim1, minDim2, d3));
            vertices.Add(new Vector3(maxDim1, minDim2, d3));
            vertices.Add(new Vector3(maxDim1, maxDim2, d3));
            vertices.Add(new Vector3(minDim1, maxDim2, d3));
        }

        return new TriangleMesh
        {
            Vertices = vertices,
            Faces = new List<(int f0, int f1, int f2)>
            {
                (2, 1, 0), (3, 2, 0)
            },
            Texture = new List<(float texU, float texV)>
            {
                (0, 0),
                (1, 0),
                (1, 1),
                (0, 1)
            }
        };
    }
}