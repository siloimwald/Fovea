using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.VectorMath;
using Xunit;

namespace Fovea.Tests
{
    public class CylinderHitTests
    {
        [Fact]
        public void HitCapFromOutSide()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            // shoot looking from +z to -z
            var rayFromPosZ = new Ray(new Point3(0, 0, 2), new Vec3(0, 0, -1));
            var hitRecord = new HitRecord();

            var hasHit = cyl.Hit(rayFromPosZ, 1e-4, 1e12, ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Point3(0, 0, 1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vec3(0, 0, 1), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);

            // other side
            var rayFromNegZ = new Ray(new Point3(0, 0, -2), new Vec3(0, 0, 1));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromNegZ, 1e-4, 1e12, ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Point3(0, 0, -1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vec3(0, 0, -1), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);
        }

        [Fact]
        public void HitCapFromInside()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            // shoot looking from origin
            var rayToNegZ = new Ray(new Point3(), new Vec3(0, 0, -1));
            var hitRecord = new HitRecord();

            var hasHit = cyl.Hit(rayToNegZ, 1e-4, 1e12, ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Point3(0, 0, -1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vec3(0, 0, 1), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);

            // same again, other direction
            var rayToPosZ = new Ray(new Point3(), new Vec3(0, 0, 1));
            hitRecord = new HitRecord();

            hasHit = cyl.Hit(rayToPosZ, 1e-4, 1e12, ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Point3(0, 0, 1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vec3(0, 0, -1), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);
        }

        [Fact]
        public void BodyHitFromOutSide()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            // half way on the pos z side from -2
            var rayFromMinusX = new Ray(new Point3(-2, 0, 0.5), new Vec3(1, 0, 0));
            var hitRecord = new HitRecord();
            var hasHit = cyl.Hit(rayFromMinusX, 1e-4, 1e12, ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Point3(-1, 0, 0.5), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vec3(-1, 0, 0), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);

            var rayFromPosY = new Ray(new Point3(0, 2, -0.5), new Vec3(0, -1, 0));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromPosY, 1e-4, 1e12, ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Point3(0, 1, -0.5), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vec3(0, 1, 0), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);
        }

        [Fact]
        public void BodyHitFromInside()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            var rayFromPosZ = new Ray(new Point3(0, 0, 0.5), new Vec3(1, 0, 0));
            var hitRecord = new HitRecord();
            var hasHit = cyl.Hit(rayFromPosZ, 1e-4, 1e12, ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Point3(1, 0, 0.5), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vec3(-1, 0, 0), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);

            var rayFromPosY = new Ray(new Point3(0, 0, -0.5), new Vec3(0, -1, 0));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromPosY, 1e-4, 1e12, ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Point3(0, -1, -0.5), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vec3(0, 1, 0), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);
        }

        [Fact]
        public void TestCylinderBox()
        {
            var cyl = new Cylinder(-5, 1, 5, null);
            var box = cyl.GetBoundingBox(0, 1);
            Assert.Equal(new Vec3(10, 10, 6), box.GetExtent());
            Assert.Equal(new Point3(0, 0, -2), box.GetCentroid());
        }
    }
}