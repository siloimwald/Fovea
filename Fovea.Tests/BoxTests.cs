using System.Numerics;
using Fovea.Renderer.Primitives;
using Xunit;

namespace Fovea.Tests
{
    public class BoxTests
    {
        [Fact]
        public void TestSphereBox()
        {
            var s = new Sphere(new Vector3(1, 1, 1), 2, null);
            var bBox = s.GetBoundingBox(0, 1);
            Assert.Equal(6 * 4 * 4, bBox.GetArea(), 3);
            Assert.Equal(new Vector3(1, 1, 1), bBox.GetCentroid());
        }
    }
}