using System;
using FluentAssertions;
using Fovea.Renderer.Core;
using NUnit.Framework;

namespace Fovea.Tests;

public class MiscTests
{
    // verifies that our image partitioning scheme touches every pixel exactly once
    
    [TestCase(16, 320, 200, 10)]
    [TestCase(7, 812, 217, 200)]
    [TestCase(40, 16, 16, 100)]
    public void TestPartitioningScheme(int threadCount, int imageWidth, int imageHeight, int pixelPerThread)
    {
        var totalPixels = imageWidth * imageHeight;
        var image = new int[totalPixels];

        for (var t = 0; t < threadCount; t++)
        {
            var threadGlobalStart = t * pixelPerThread;
            var inc = pixelPerThread * threadCount;
            for (var offset = threadGlobalStart; offset < totalPixels; offset += inc)
            {
                var max = Math.Min(totalPixels, offset + pixelPerThread);
                for (var p = offset; p < max; p++)
                    image[p] += 1;
            }
        }

        image.Should().AllSatisfy(p => p.Should().Be(1));
    }

    [TestCase(-0.5f, 1.5f, 0.5f, 2.0f, -0.5f, 2.0f)]
    [TestCase(1f, 2f, -2f, -1f, -2f, 2f)]
    public void IntervalUnion(float leftMin, float leftMax, float rightMin, float rightMax, float unionMin, float unionMax)
    {
        var a = new Interval(leftMin, leftMax);
        var b = new Interval(rightMin, rightMax);
        var union = new Interval(a, b);
        union.Min.Should().Be(unionMin);
        union.Max.Should().Be(unionMax);
    }
}