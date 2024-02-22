using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Primitives;
using NUnit.Framework;


namespace Fovea.Tests;

public class CylinderHitTests
{
    [Test]
    public void HitCapFromOutSide()
    {
        // cylinder on z axis from -1 to 1 with radius 1
        var cyl = new Cylinder(-1, 1, 1, null);

        // shoot looking from +z to -z
        var rayFromPosZ = new Ray(new Vector3(0, 0, 2), new Vector3(0, 0, -1));
        var hitRecord = new HitRecord();

        var hasHit = cyl.Hit(rayFromPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);
        Assert.True(hasHit);
        hitRecord.HitPoint.Should().Be(Vector3.UnitZ);
        hitRecord.RayT.Should().Be(1); // from z=2 to z=1 at bottom cap
        hitRecord.Normal.Should().Be(Vector3.UnitZ);
        hitRecord.IsFrontFace.Should().BeTrue();
        
        // other side
        var rayFromNegZ = new Ray(new Vector3(0, 0, -2), new Vector3(0, 0, 1));
        hitRecord = new HitRecord();
        hasHit = cyl.Hit(rayFromNegZ, new Interval(1e-4f, 1e12f), ref hitRecord);

        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(-Vector3.UnitZ);
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(-Vector3.UnitZ);
        hitRecord.IsFrontFace.Should().BeTrue();
    }

    [Test]
    public void HitCapFromInside()
    {
        // cylinder on z axis from -1 to 1 with radius 1
        var cyl = new Cylinder(-1, 1, 1, null);

        // shoot looking from origin
        var rayToNegZ = new Ray(new Vector3(), new Vector3(0, 0, -1));
        var hitRecord = new HitRecord();

        var hasHit = cyl.Hit(rayToNegZ, new Interval(1e-4f, 1e12f), ref hitRecord);
        
        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(-Vector3.UnitZ);
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(Vector3.UnitZ);
        hitRecord.IsFrontFace.Should().BeFalse();
        
        // same again, other direction
        var rayToPosZ = new Ray(new Vector3(), new Vector3(0, 0, 1));
        hitRecord = new HitRecord();

        hasHit = cyl.Hit(rayToPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);
        
        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(Vector3.UnitZ);
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(-Vector3.UnitZ);
        hitRecord.IsFrontFace.Should().BeFalse();
        
    }

    [Test]
    public void BodyHitFromOutSide()
    {
        // cylinder on z axis from -1 to 1 with radius 1
        var cyl = new Cylinder(-1, 1, 1, null);

        // half way on the pos z side from -2
        var rayFromMinusX = new Ray(new Vector3(-2, 0, 0.5f), new Vector3(1, 0, 0));
        var hitRecord = new HitRecord();
        var hasHit = cyl.Hit(rayFromMinusX, new Interval(1e-4f, 1e12f), ref hitRecord);
        
        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(new Vector3(-1, 0, 0.5f));
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(-Vector3.UnitX);
        hitRecord.IsFrontFace.Should().BeTrue();

        var rayFromPosY = new Ray(new Vector3(0, 2, -0.5f), new Vector3(0, -1, 0));
        hitRecord = new HitRecord();
        hasHit = cyl.Hit(rayFromPosY, new Interval(1e-4f, 1e12f), ref hitRecord);
        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(new Vector3(0, 1, -0.5f));
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(Vector3.UnitY);
        hitRecord.IsFrontFace.Should().BeTrue();
    }

    [Test]
    public void BodyHitFromInside()
    {
        // cylinder on z axis from -1 to 1 with radius 1
        var cyl = new Cylinder(-1, 1, 1, null);

        var rayFromPosZ = new Ray(new Vector3(0, 0, 0.5f), new Vector3(1, 0, 0));
        var hitRecord = new HitRecord();
        var hasHit = cyl.Hit(rayFromPosZ, new Interval(1e-4f, 1e12f), ref hitRecord);

        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(new Vector3(1, 0, 0.5f));
        hitRecord.RayT.Should().Be(1.0f);
        hitRecord.Normal.Should().Be(-Vector3.UnitX);
        hitRecord.IsFrontFace.Should().BeFalse();
        
        var rayFromPosY = new Ray(new Vector3(0, 0, -0.5f), new Vector3(0, -1, 0));
        hitRecord = new HitRecord();
        hasHit = cyl.Hit(rayFromPosY, new Interval(1e-4f, 1e12f), ref hitRecord);
        hasHit.Should().BeTrue();
        hitRecord.HitPoint.Should().Be(new Vector3(0, -1, -0.5f));
        hitRecord.RayT.Should().Be(1);
        hitRecord.Normal.Should().Be(Vector3.UnitY);
        hitRecord.IsFrontFace.Should().BeFalse();
    }

    [Test]
    public void TestCylinderBox()
    {
        var cyl = new Cylinder(-5, 1, 5, null);
        var box = cyl.GetBoundingBox();
        box.GetExtent().Should().Be(new Vector3(10, 10, 6));
        box.GetCentroid().Should().Be(new Vector3(0, 0, -2));
    }
}