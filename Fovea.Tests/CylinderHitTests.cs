using System.Numerics;
using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;
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
            var rayFromPosZ = new Ray(new Vector3(0, 0, 2), new Vector3(0, 0, -1));
            var hitRecord = new HitRecord();

            var hasHit = cyl.Hit(rayFromPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, 0, 1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vector3(0, 0, 1), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);

            // other side
            var rayFromNegZ = new Ray(new Vector3(0, 0, -2), new Vector3(0, 0, 1));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromNegZ, new Interval(1e-4f, 1e12f), ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, 0, -1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vector3(0, 0, -1), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);
        }

        [Fact]
        public void HitCapFromInside()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            // shoot looking from origin
            var rayToNegZ = new Ray(new Vector3(), new Vector3(0, 0, -1));
            var hitRecord = new HitRecord();

            var hasHit = cyl.Hit(rayToNegZ, new Interval(1e-4f, 1e12f), ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, 0, -1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vector3(0, 0, 1), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);

            // same again, other direction
            var rayToPosZ = new Ray(new Vector3(), new Vector3(0, 0, 1));
            hitRecord = new HitRecord();

            hasHit = cyl.Hit(rayToPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, 0, 1), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3); // from z=2 to z=1 at bottom cap
            Assert.Equal(new Vector3(0, 0, -1), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);
        }

        [Fact]
        public void BodyHitFromOutSide()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            // half way on the pos z side from -2
            var rayFromMinusX = new Ray(new Vector3(-2, 0, 0.5f), new Vector3(1, 0, 0));
            var hitRecord = new HitRecord();
            var hasHit = cyl.Hit(rayFromMinusX, new Interval(1e-4f, 1e12f), ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Vector3(-1, 0, 0.5f), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vector3(-1, 0, 0), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);

            var rayFromPosY = new Ray(new Vector3(0, 2, -0.5f), new Vector3(0, -1, 0));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromPosY, new Interval(1e-4f, 1e12f), ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, 1, -0.5f), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vector3(0, 1, 0), hitRecord.Normal);
            Assert.True(hitRecord.IsFrontFace);
        }

        [Fact]
        public void BodyHitFromInside()
        {
            // cylinder on z axis from -1 to 1 with radius 1
            var cyl = new Cylinder(-1, 1, 1, null);

            var rayFromPosZ = new Ray(new Vector3(0, 0, 0.5f), new Vector3(1, 0, 0));
            var hitRecord = new HitRecord();
            var hasHit = cyl.Hit(rayFromPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);

            Assert.True(hasHit);
            Assert.Equal(new Vector3(1, 0, 0.5f), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vector3(-1, 0, 0), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);

            var rayFromPosY = new Ray(new Vector3(0, 0, -0.5f), new Vector3(0, -1, 0));
            hitRecord = new HitRecord();
            hasHit = cyl.Hit(rayFromPosY, new Interval(1e-4f, 1e12f), ref hitRecord);
            Assert.True(hasHit);
            Assert.Equal(new Vector3(0, -1, -0.5f), hitRecord.HitPoint);
            Assert.Equal(1.0, hitRecord.RayT, 3);
            Assert.Equal(new Vector3(0, 1, 0), hitRecord.Normal);
            Assert.False(hitRecord.IsFrontFace);
        }

        [Fact]
        public void TestCylinderBox()
        {
            var cyl = new Cylinder(-5, 1, 5, null);
            var box = cyl.GetBoundingBox(0, 1);
            Assert.Equal(new Vector3(10, 10, 6), box.GetExtent());
            Assert.Equal(new Vector3(0, 0, -2), box.GetCentroid());
        }
    }
}