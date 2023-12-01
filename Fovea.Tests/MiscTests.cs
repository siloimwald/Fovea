using System;
using FluentAssertions;
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
}