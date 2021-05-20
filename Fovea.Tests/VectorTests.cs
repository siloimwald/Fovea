using Fovea.Renderer.VectorMath;
using Xunit;
using static System.Math;

namespace Fovea.Tests
{
    public class VectorTests
    {
        [Fact]
        public void VectorAddition()
        {
            var v = new Vec3(2, -4, 1);
            var w = new Vec3(-2, 0f, 14);
            Assert.Equal(new Vec3(0, -4, 15), v+w);
        }

        [Fact]
        public void VectorScalarMultiplication()
        {
            var v = new Vec3(4, 1, -3);
            Assert.Equal(new Vec3(2, 0.5f, -1.5f), v*0.5f);
        }

        [Fact]
        public void VectorSubtraction()
        {
            var v = new Vec3(2.9f, -3.0f, 0);
            var w = new Vec3(2.9f, -6.5f, 2);
            Assert.Equal(new Vec3(0, 3.5f, -2f), v-w);
        }

        [Fact] // for sake of completeness
        public void VectorUnaryMinus()
        {
            var v = new Vec3(-3, 14, 23);
            Assert.Equal(new Vec3(3, -14, -23), -v);
        }

        // common up with useful test cases is hard
        [Fact]
        public void LengthWorks()
        {
            var p0 = new Point3(20, -10, 40);
            var p1 = new Point3(40, 10, 60);
            var v = p1 - p0;
            var len0 = v.Length();
            Assert.Equal(v.LengthSquared(), len0 * len0, 3);
            Assert.Equal(len0*2, (v+v).Length(), 3);
        }

        [Fact]
        public void DotWorks()
        {
            var v = new Vec3(14, 0, -20);
            Assert.Equal(v.LengthSquared(), Vec3.Dot(v,v), 3);
        }

        [Fact]
        public void NormalizeWorks()
        {
            var w = new Vec3(14, 9, -3);
            Assert.Equal(1.0f, Vec3.Normalize(w).Length(), 3);
        }

        [Fact]
        public void CrossWorks()
        {
            var w = new Vec3(9, 2, 14);
            var v = new Vec3(-3, 22, 3);
            var c = Vec3.Cross(w, v);
            // c is perpendicular to both w and v
            Assert.Equal(0.0f, Vec3.Dot(c, v), 3);
            Assert.Equal(0.0f, Vec3.Dot(c, w), 3);
        }

        [Fact]
        public void ReflectWorks()
        {
            var n = new Vec3(0, 1, 0);
            var v = new Vec3(-2, 1, 14);
            var r = Vec3.Reflect(v, n);
            // r and v have the same angle (dot product) to n, just with different signs
            Assert.Equal(Abs(Vec3.Dot(r,n)), Abs(Vec3.Dot(v,n)), 3);
        }
    }
}