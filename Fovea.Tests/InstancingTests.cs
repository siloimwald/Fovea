using System.Collections.Generic;
using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Primitives;
using NUnit.Framework;

namespace Fovea.Tests;

public class InstancingTests
{
    [Test]
    public void QuadInstance()
    {
        // quad at [-1,-1,0] to [1,1,0] in z plane
        var blueprint = new Quad(new Vector3(-1, -1, 0),
            new Vector3(2, 0, 0), new Vector3(0, 2, 0), null);

        var hr = new HitRecord();
        // straight onto the z plane
        var ray = new Ray(
            new Vector3(-0.5f, -0.25f, -5),
            Vector3.UnitZ
        );

        var blueprintHit = blueprint.Hit(ray, new Interval(1e-3f, 8000f), ref hr);
        blueprintHit.Should().BeTrue();
        
        // move it so corner is at origin
        List<ITransformationDescriptor> transforms =
        [
            new TranslationDescriptor
            {
                X = 1,
                Y = 1
            }
        ];

        var forward = transforms.GetTransformation();
        var couldInvert = Matrix4x4.Invert(forward, out var inverse);
        couldInvert.Should().BeTrue(); // that one should be trivially true for this test
        var instance = new Instance(blueprint, forward, inverse, null);
        
        ray = new Ray(
            new Vector3(0.5f, 0.25f, -5),
            Vector3.UnitZ
        );
        hr = new HitRecord(); // reset stuff
        var boxFoo = instance.GetBoundingBox();
        var hasHit = instance.Hit(ray, new Interval(1e-3f, float.PositiveInfinity), ref hr);
        
        hasHit.Should().BeTrue();
    }
}