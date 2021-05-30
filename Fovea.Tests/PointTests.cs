using Fovea.Renderer.VectorMath;
using Xunit;

namespace Fovea.Tests
{
    public class PointTests
    {
        [Fact]
        public void PointPlusMinusVector()
        {
            var p = new Point3(3, 9, -3);
            var v = new Vec3(2, -3, 10);
            Assert.Equal(new Point3(5, 6, 7), p + v);
            Assert.Equal(p, (p + v) - v);
        }

        [Fact]
        public void PointMinusPoint()
        {
            var p0 = new Point3(2, -4, 6);
            var p1 = new Point3(4, -10, 23.1f);
            var v = p0 - p1;
            Assert.Equal(new Vec3(-2, 6, -17.1f), v);
        }

        [Fact]
        public void PointMax()
        {
            var p0 = new Point3(-10, 20, 0);
            var p1 = new Point3(22, 20, 1);
            var pMax = Point3.Max(p0, p1);
            Assert.Equal(new Point3(22, 20, 1), pMax);
        }

        [Fact]
        public void PointMin()
        {
            var p0 = new Point3(-10, 20, 0);
            var p1 = new Point3(22, 20, 1);
            var pMin = Point3.Min(p0, p1);
            Assert.Equal(new Point3(-10, 20, 0), pMin);
        }
    }
}