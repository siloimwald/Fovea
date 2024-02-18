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
        sphereDescriptor.MaterialReference.Should().Be("foo");
    }

    // attempt at parsing a arbitrary box (made up of 6 faces with 2 triangles each)
    [Test]
    public void QuadParsing()
    {
        const string yaml = """
                            extentMin: { x: -20, y: 10.4 }
                            extentMax: { x: 4, y: -69.42 }
                            axis: 'bla'
                            position: 4
                            material: 'yellow'
                            flipNormals: true
                            asMesh: true
                            """;
        var deserializer = YamlParser.GetDeserializer();
        var quadDescriptor = deserializer.Deserialize<QuadDescriptor>(yaml);
        quadDescriptor.Axis.Should().Be("bla");
        quadDescriptor.ExtentMin.Should().Be(new Vector2(-20f, 10.4f));
        quadDescriptor.Position.Should().Be(4);
        quadDescriptor.ExtentMax.Should().Be(new Vector2(4f, -69.42f));
        quadDescriptor.MaterialReference.Should().Be("yellow");
        quadDescriptor.FlipNormals.Should().BeTrue();
        quadDescriptor.AsMesh.Should().BeTrue();
    }

    [Test]
    public void CompoundSceneList()
    {
        const string yaml = """
                            [
                             !quad { extentMin: { x: -20, y: 10.4 }, extentMax: { x: 4, y: -69.42 }, axis: 'bla' },
                             !sphere { center: { x: 128, y: -1, z: -10.12 }, radius: 42.69 }
                            ]
                            """;
        var deserializer = YamlParser.GetDeserializer();
        var scene = deserializer.Deserialize<List<IPrimitiveGenerator>>(yaml);
        scene.Should().HaveCount(2);
        scene[0].Should().BeOfType<QuadDescriptor>();
        scene[1].Should().BeOfType<SphereDescriptor>();
        (scene[0] as QuadDescriptor)!.Axis.Should().Be("bla");
        (scene[1] as SphereDescriptor)!.Radius.Should().Be(42.69f);
    }

    [Test]
    public void CompoundTextureList()
    {
        const string yaml = """
                            [
                                !ct { color: { r: 1, g: 0.5, b: 1 } },
                                !t { fileName: "foo.png" },
                                !noise { scale: 1.5 }
                            ]
                            """;

        var deserializer = YamlParser.GetDeserializer();
        var textureList = deserializer.Deserialize<List<ITextureGenerator>>(yaml);
        textureList.Should().HaveCount(3);
        textureList[0].Should().BeOfType<ColorTextureDescriptor>();
        textureList[1].Should().BeOfType<ImageTextureDescriptor>();
        textureList[2].Should().BeOfType<NoiseTextureDescriptor>();
        (textureList[0] as ColorTextureDescriptor)!.Color.Should().Be(new RGBColor(1, 0.5f, 1));
        (textureList[1] as ImageTextureDescriptor)!.FileName.Should().Be("foo.png");
        (textureList[2] as NoiseTextureDescriptor)!.Scale.Should().Be(1.5f);
    }

    [Test]
    public void RenderOptionsParser()
    {
        const string yaml = """
                            {   numSamples: 500,
                                imageWidth: 320,
                                imageHeight: 200,
                                maxDepth: 50,
                                outputFile: 'result.png'
                            }
                            """;

        var deserializer = YamlParser.GetDeserializer();
        var opts = deserializer.Deserialize<RenderOptions>(yaml);
        opts.NumSamples.Should().Be(500);
        opts.ImageWidth.Should().Be(320);
        opts.ImageHeight.Should().Be(200);
        opts.MaxDepth.Should().Be(50);
        opts.OutputFile.Should().Be("result.png");

        // partial defaults are preserved
        const string yaml2 = """
                             {   numSamples: 500,
                                 imageHeight: 200
                             }
                             """;

        opts = deserializer.Deserialize<RenderOptions>(yaml2);
        opts.NumSamples.Should().Be(500);
        opts.ImageWidth.Should().Be(800);
        opts.ImageHeight.Should().Be(200);
        opts.MaxDepth.Should().Be(50);
    }

    [Test]
    public void MatteMaterialParsing()
    {
        const string yaml = "{ texture: 'whatever' }";
        var deserializer = YamlParser.GetDeserializer();
        var matteDescriptor = deserializer.Deserialize<MatteDescriptor>(yaml);
        matteDescriptor.TextureReference.Should().Be("whatever");
    }

    [Test]
    public void MetalMaterialParsing()
    {
        const string yaml = "{ texture: 'bla', fuzzy: 0.35 }";
        var deserializer = YamlParser.GetDeserializer();
        var metalDescriptor = deserializer.Deserialize<MetalDescriptor>(yaml);
        metalDescriptor.TextureReference.Should().Be("bla");
        metalDescriptor.Fuzzy.Should().Be(0.35f);
    }

    [Test]
    public void SimpleSceneParsing()
    {
        var deserializer = YamlParser.GetDeserializer();
        var scene = deserializer.Deserialize<SceneDescriptor>(SimpleSceneYaml);

        scene.Materials.Should().ContainKeys("blue", "green_metal");
        scene.Textures.Should().ContainKeys("blue", "greenish");
        scene.Materials["green_metal"].Should().BeOfType<MetalDescriptor>();
        var greenMetal = scene.Materials["green_metal"] as MetalDescriptor;
        greenMetal!.Fuzzy.Should().Be(0.5f);
        greenMetal.TextureReference.Should().Be("greenish");
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

    [Test]
    public void DiffuseLightParsing()
    {
        const string yaml = "!diffLight { texture: 'fooWhite' }";
        var deserializer = YamlParser.GetDeserializer();
        var light = deserializer.Deserialize<DiffuseLightDescriptor>(yaml);
        light.TextureReference.Should().Be("fooWhite");
    }

    [Test]
    public void LightSourceHandling()
    {
        // a light source should be correctly parsed and also end up in the light source block
        const string yaml = """
                            camera:
                                near: 1
                                far: 10
                            textures:
                                white: !ct 
                                    color:
                                     r: 1
                                     g: 1
                                     b: 1
                            materials:
                                light: !matte
                                    texture: white
                            lights:
                                - !sphere
                                  center:
                                    y: 1
                                  material: light                                       
                            """;
        
        var deserializer = YamlParser.GetDeserializer();
        var scene = deserializer.Deserialize<SceneDescriptor>(yaml);
        scene.Lights.Should().HaveCount(1);
        scene.Lights[0].Should().BeOfType<SphereDescriptor>();
        var buildResult = scene.Build(new ParserContext());
        buildResult.Lights.Should().BeOfType<PrimitiveList>();
        (buildResult.Lights as PrimitiveList)![0].Should().BeOfType<Sphere>();
    }
}