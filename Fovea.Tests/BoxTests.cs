using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Primitives;
using NUnit.Framework;

namespace Fovea.Tests;

public class BoxTests
{
    [Test]
    public void TestSphereBox()
    {
        var s = new Sphere(new Vector3(1, 1, 1), 2, null);
        var bBox = s.GetBoundingBox(0, 1);
        bBox.GetArea().Should().Be(6 * 4 * 4);
        bBox.GetCentroid().Should().Be(Vector3.One);
    }
}