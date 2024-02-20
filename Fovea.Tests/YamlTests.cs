using System.Collections.Generic;
using System.Numerics;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Parser.Yaml;
using Fovea.Renderer.Primitives;
using NUnit.Framework;

namespace Fovea.Tests;

public class YamlTests
{
    private const string SimpleSceneYaml = """
                                           options:
                                              samples: 250
                                           materials:
                                              blue:
                                                  !matte
                                                      texture: "blue"
                                              green_metal:
                                                  !metal { texture: "greenish",
                                                           fuzzy: 0.5 }
                                           textures:
                                              blue: !ct { color: { r: 0.3, g: 0.2, b:1 } }
                                              greenish: !ct { color: { r: 0.2, g: 0.8, b: 0.3 } }
                                           primitives:
                                              - !sphere
                                                  center:
                                                      y: 1
                                                  radius: 0.5
                                                  material: "blue"
                                              - !quad
                                                  extentMin: { x: -10, y: 10 }
                                                  extentMax: { x: 10, y: -10 }
                                                  material: "greenish"
                                           """;


    [Test]
    public void CameraParsing()
    {
        const string yaml = """
                            orientation:
                              lookAt: { x: 278, y: -10.5, z: 42 }
                              lookFrom:
                                 y: -3
                                 z: 9
                              upDirection:
                                 y: 1
                            fieldOfView: 40
                            focusDistance: 4.5
                            defocusAngle: 2.4
                            """;

        var deserializer = YamlParser.GetDeserializer();

        var cameraDescriptor = deserializer.Deserialize<CameraDescriptor>(yaml);

        cameraDescriptor.FocusDistance.Should().Be(4.5f);
        cameraDescriptor.DefocusAngle.Should().Be(2.4f);
        cameraDescriptor.FieldOfView.Should().Be(40);
        cameraDescriptor.Orientation.LookAt.Should().Be(new Vector3(278, -10.5f, 42));
        cameraDescriptor.Orientation.LookFrom.Should().Be(new Vector3(0, -3, 9));
        cameraDescriptor.Orientation.UpDirection.Should().Be(Vector3.UnitY);
    }

    [Test]
    public void CameraParsingDefaults()
    {
        const string yaml = "fieldOfView: 40";

        var defaultCam = new CameraDescriptor();
        var deserializer = YamlParser.GetDeserializer();
        var cameraDescriptor = deserializer.Deserialize<CameraDescriptor>(yaml);

        cameraDescriptor.FocusDistance.Should().Be(defaultCam.FocusDistance);
        cameraDescriptor.DefocusAngle.Should().Be(defaultCam.DefocusAngle);
        cameraDescriptor.FieldOfView.Should().Be(40);
        cameraDescriptor.Orientation.LookAt.Should().Be(defaultCam.Orientation.LookAt);
        cameraDescriptor.Orientation.LookFrom.Should().Be(defaultCam.Orientation.LookFrom);
        cameraDescriptor.Orientation.UpDirection.Should().Be(defaultCam.Orientation.UpDirection);
    }


    [Test]
    public void SphereParsing()
    {
        const string yaml = """{ center: {x: 128, y: -1, z: -10.12 } material: "foo" , radius: 42.69 }""";

        var deserializer = YamlParser.GetDeserializer();
        var sphereDescriptor = deserializer.Deserialize<SphereDescriptor>(yaml);
        sphereDescriptor.Radius.Should().BeApproximately(42.69f, 1e-4f);
        sphereDescriptor.Center.Should().Be(new Vector3(128, -1, -10.12f));
        sphereDescriptor.Center1.Should().Be(null);
        sphereDescriptor.IsMoving.Should().BeFalse();
        sphereDescriptor.MaterialReference.Should().Be("foo");
        
        const string movingSphere = """
                                    center: {x: 1, y: 0, z: 1 }
                                    center1: { x: 2, y: 4, z: -2 }
                                    material: "foo"
                                    radius: 42.69 
                                    """;
        var movingSphereDescriptor = deserializer.Deserialize<SphereDescriptor>(movingSphere);
        movingSphereDescriptor.IsMoving.Should().BeTrue();
        movingSphereDescriptor.Center1.Should().Be(new Vector3(2, 4, -2));
    }   

    [Test]
    public void DeserializeKeepsTags()
    {
        const string yaml = """
                            [
                             !quad { extentMin: { x: -20, y: 10.4 }, extentMax: { x: 4, y: -69.42 }, axis: 'bla' },
                             !sphere { center: { x: 128, y: -1, z: -10.12 }, radius: 42.69 }
                            ]
                            """;
        var scene = YamlParser.GetDeserializer().Deserialize<List<IPrimitiveGenerator>>(yaml);
        var serializer = YamlParser.GetSerializer();
        var serializedYaml = serializer.Serialize(scene);
        var roundTrip = YamlParser.GetDeserializer().Deserialize<List<IPrimitiveGenerator>>(serializedYaml);
        roundTrip.Should().HaveCount(2);
        roundTrip[0].Should().BeOfType<QuadDescriptor>();
        roundTrip[1].Should().BeOfType<SphereDescriptor>();
        (roundTrip[1] as SphereDescriptor)!.Radius.Should().Be(42.69f);
    }

    [Test]
    public void FlipFaceParsing()
    {
        const string yaml = "!ff { primitive: !sphere { center: { x: 128, y: -1, z: -10.12 }, radius: 42.69 } }";
        var deserializer = YamlParser.GetDeserializer();
        var flipFace = deserializer.Deserialize<FlipFaceDescriptor>(yaml);
        flipFace.Should().BeOfType<FlipFaceDescriptor>();
        flipFace.Primitive.Should().BeOfType<SphereDescriptor>();
        (flipFace.Primitive as SphereDescriptor)!.Radius.Should().Be(42.69f);
    }

    [Test]
    public void MeshFileParsing()
    {
        const string yaml = "!mesh { fileName: 'foo.obj', normalize: true, vertexNormals: true, flipNormals: true }";
        var deserializer = YamlParser.GetDeserializer();
        var mesh = deserializer.Deserialize<MeshFileDescriptor>(yaml);
        mesh.FileName.Should().Be("foo.obj");
        mesh.VertexNormals.Should().BeTrue();
        mesh.FlipNormals.Should().BeTrue();
        mesh.Normalize.Should().BeTrue();
    }


}