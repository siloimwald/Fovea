using FluentAssertions;
using Fovea.Renderer.Sampling;
using NUnit.Framework;
using static System.MathF;

namespace Fovea.Tests;

public class SamplerTests
{
    [Test]
    public void TestRandomUnitSphere()
    {
        // meh...
        for (var i = 0; i < 100; i++)
        {
            var v = Sampler.Instance.RandomOnUnitSphere();
            // is normal and abs of each component is <= 1
            v.Length().Should().BeApproximately(1, 0.0001f);
            Abs(v.X).Should().BeLessThanOrEqualTo(1);
            Abs(v.Y).Should().BeLessThanOrEqualTo(1);
            Abs(v.Z).Should().BeLessThanOrEqualTo(1);
        }
    }

    [Test]
    public void TestRandomUnitDisk()
    {
        for (var i = 0; i < 100; i++)
        {
            var (x, y) = Sampler.Instance.RandomOnUnitDisk();
            x.Should().BeLessOrEqualTo(1);       
            y.Should().BeLessOrEqualTo(1);
            (x * x + y * y).Should().BeLessThanOrEqualTo(1);
        }
    }
}