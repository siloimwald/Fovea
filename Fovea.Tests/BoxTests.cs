using Fovea.Renderer.Primitives;
using Fovea.Renderer.VectorMath;
using Xunit;

namespace Fovea.Tests
{
    public class BoxTests
    {
        [Fact]
        public void TestSphereBox()
        {
            var s = new Sphere(new Point3(1, 1, 1), 2, null);
            var bBox = s.GetBoundingBox(0, 1);
            Assert.Equal(6 * 4 * 4, bBox.GetArea(), 3);
            Assert.Equal(new Point3(1, 1, 1), bBox.GetCentroid());
        }
    }
}