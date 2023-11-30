using System;
using System.Linq;
using Xunit;

namespace Fovea.Tests;

public class MiscTests
{
    [Fact]
    public void TestPartitioningScheme()
    {
        var threadCount = 13;
        var imageWidth = 358;
        var imageHeight = 479;

        var totalPixels = imageWidth * imageHeight;
        var image = new int[totalPixels];
        var pixelPerThread = 10;

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

        Assert.True(image.All(pixel => pixel == 1));
    }
}