using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Primitives;
using NUnit.Framework;

namespace Fovea.Tests;

public class BoxTests
{
    [Test]
    public void SphereBox()
    {
        var s = new Sphere(new Vector3(1, 1, 1), 2, null);
        var bBox = s.GetBoundingBox();
        bBox.GetArea().Should().Be(6 * 4 * 4);
        bBox.GetCentroid().Should().Be(Vector3.One);
    }

    [Test]
    public void MovingSphereBox()
    {
        var s = new Sphere(Vector3.One, -Vector3.One, 1, null);
        var box = s.GetBoundingBox();
        box.Min.Should().Be(new Vector3(-2, -2, -2));
        box.Max.Should().Be(new Vector3(2, 2, 2));
    }
}