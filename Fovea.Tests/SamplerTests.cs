using Fovea.Renderer.Sampling;
using Xunit;
using static System.MathF;

namespace Fovea.Tests
{
    public class SamplerTests
    {
        [Fact]
        public void TestRandomUnitSphere()
        {
            // meh...
            for (var i = 0; i < 100; i++)
            {
                var v = Sampler.Instance.RandomOnUnitSphere();
                // is normal and abs of each component is <= 1
                Assert.Equal(1.0, v.Length(), 3);
                Assert.True(Abs(v.X) <= 1);
                Assert.True(Abs(v.Y) <= 1);
                Assert.True(Abs(v.Z) <= 1);
            }
        }

        [Fact]
        public void TestRandomUnitDisk()
        {
            for (var i = 0; i < 100; i++)
            {
                var (x, y) = Sampler.Instance.RandomOnUnitDisk();
                Assert.True(x <= 1);
                Assert.True(y <= 1);
                Assert.True(x * x + y * y <= 1);
            }
        }
    }
}