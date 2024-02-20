using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;
using NUnit.Framework;

namespace Fovea.Tests;

public class QuadHitTests
{
    // very simplistic test, just hit straight from above, barely inside, barely outside
    [TestCase(-.99f, 5, -0.99f, true)]
    [TestCase(-.99f, 5, 0.99f, true)]
    [TestCase(.99f, 5, -0.99f, true)]
    [TestCase(.99f, 5, 0.99f, true)]
    [TestCase(.99f, 5, -1.01f, false)]
    [TestCase(1.01f, 5, 0.99f, false)]
    public void HitQuadTest(float orgX, float orgY, float orgZ, bool shouldHit)
    {
        var quad = new Quad(new Vector3(-1, 0, -1), Vector3.UnitZ * 2, Vector3.UnitX * 2, null);

        var hr = new HitRecord();
        var hit = quad.Hit(new Ray(new Vector3(orgX, orgY, orgZ), -Vector3.UnitY), new Interval(1e-3f, float.PositiveInfinity),
            ref hr);
        hit.Should().Be(shouldHit);
    }
}