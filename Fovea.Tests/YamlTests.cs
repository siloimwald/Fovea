using System;
using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Parser;
using NUnit.Framework;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fovea.Tests;

public class YamlTests
{
    [Test]
    public void CameraParsing()
    {
        const string yaml = """
                       orientation:
                         look_at: !3 [278, -10.5, 42]
                         look_from: !3 [0,-3,9]
                         up_direction: !3 [0,1,0]
                       field_of_view: 40
                       near: 1
                       far: 1500
                       """;

        var deserializer = GetDeserializer();
           
        var cameraDescriptor = deserializer.Deserialize<CameraDescriptor>(yaml);

        cameraDescriptor.Far.Should().Be(1500);
        cameraDescriptor.Near.Should().Be(1);
        cameraDescriptor.FieldOfView.Should().Be(40);
        cameraDescriptor.Orientation.LookAt.Should().Be(new Vector3(278, -10.5f, 42));
        cameraDescriptor.Orientation.LookFrom.Should().Be(new Vector3(0, -3, 9));
        cameraDescriptor.Orientation.UpDirection.Should().Be(Vector3.UnitY);
    }

    [Test]
    public void SphereParsing()
    {
        const string yaml = "{ center: !3 [128, -1, -10.12], radius: 42.69 }";
        
        var deserializer = GetDeserializer();
        var sphereDescriptor = deserializer.Deserialize<SphereDescriptor>(yaml);
        sphereDescriptor.Radius.Should().BeApproximately(42.69f, 1e-4f);
        sphereDescriptor.Center.Should().Be(new Vector3(128, -1, -10.12f));
    }

    private static IDeserializer GetDeserializer()
    {
        return new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .WithTypeConverter(new Vector3YamlConverter())
            .WithTagMapping(new TagName("!3"), typeof(Vector3))
            .Build();

    }
}