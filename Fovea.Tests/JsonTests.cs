using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Parser.Yaml;
using NUnit.Framework;

namespace Fovea.Tests;

public class JsonTests
{
    private const string SimpleSceneJson = """
                                           {
                                            "options": {
                                                "samples": 250
                                            },
                                           "materials": {
                                              "blue": { "$type": "matte", "texture": "blue" },
                                              "green_metal": { "$type": "metal", "texture": "greenish", "fuzzy": 0.5 }
                                            },
                                           "textures": {
                                              "blue": { "$type":"color", "r": 0.3, "g": 0.2, "b":1 },
                                              "greenish": { "$type":"color", "r": 0.2, "g": 0.8, "b": 0.3 }
                                            },
                                           "primitives": [
                                                {
                                                  "$type":"sphere",
                                                  "center": { "y": 1 },
                                                  "radius": 0.5,
                                                  "material": "blue"
                                                }
                                            ]
                                           }
                                           """;


    [Test]
    public void CameraDescriptorParsing()
    {
        const string camera = """
                              {
                                "orientation": {
                                    "lookAt": { "x": -3.4 },
                                    "lookFrom": { "z": -1.2 },
                                    "upDirection": { "y": 1.5 }
                                },
                                "fieldOfView": 35.0,
                                "defocusAngle": 0.6,
                                "focusDistance": 12,
                                "foo": 24
                              }
                              """;

        var cameraDescriptor = JsonSerializer.Deserialize<CameraDescriptor>(camera, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        });

        cameraDescriptor.Orientation.Should().NotBeNull();
        cameraDescriptor.Orientation.LookAt.Should().Be(new Vector3(-3.4f, 0, 0));
        cameraDescriptor.Orientation.LookFrom.Should().Be(new Vector3(0, 0, -1.2f));
        cameraDescriptor.Orientation.UpDirection.Should().Be(new Vector3(0, 1.5f, 0));

        cameraDescriptor.FieldOfView.Should().Be(35.0f);
        cameraDescriptor.DefocusAngle.Should().Be(0.6f);
        cameraDescriptor.FocusDistance.Should().Be(12);
    }

    [Test]
    public void SphereParsing()
    {
        const string sphereJson = """
                                  {
                                      "center": { "x": 128, "y": -1, "z": -10.12 },
                                      "material": "foo",
                                      "radius": 42.69
                                  }
                                  """;


        // var deserializer = YamlParser.GetDeserializer();
        var sphereDescriptor = JsonSerializer
            .Deserialize<SphereDescriptor>(sphereJson,
                YamlParser.JsonOptions);

        sphereDescriptor.Radius.Should().BeApproximately(42.69f, 1e-4f);
        sphereDescriptor.Center.Should().Be(new Vector3(128, -1, -10.12f));
        sphereDescriptor.Center1.Should().Be(null);
        sphereDescriptor.IsMoving.Should().BeFalse();
        sphereDescriptor.MaterialReference.Should().Be("foo");

        const string movingSphere = """
                                    {
                                        "center": { "x": 1, "y": 0, "z": 1 },
                                        "center1": { "x": 2, "y": 4, "z": -2 },
                                        "material": "foo",
                                        "radius": 42.69
                                    }
                                    """;
        var movingSphereDescriptor = JsonSerializer.Deserialize<SphereDescriptor>(movingSphere,
            YamlParser.JsonOptions);

        movingSphereDescriptor.IsMoving.Should().BeTrue();
        movingSphereDescriptor.Center1.Should().Be(new Vector3(2, 4, -2));
    }

    [Test]
    public void CompoundTextureList()
    {
        const string textureListJons =
            """
            [
                { "$type": "color", "r": 1, "g": 0.5, "b": 1 },
                { "$type": "image", "fileName": "foo.png" },
                { "$type": "noise", "scale": 1.5 }
            ]
            """;

        var textureList = JsonSerializer.Deserialize<List<ITextureGenerator>>(textureListJons,
            YamlParser.JsonOptions);

        textureList.Should().HaveCount(3);
        textureList[0].Should().BeOfType<RGBColor>();
        textureList[1].Should().BeOfType<ImageTextureDescriptor>();
        textureList[2].Should().BeOfType<NoiseTextureDescriptor>();
        textureList[0]!.Should().Be(new RGBColor(1, 0.5f, 1));
        (textureList[1] as ImageTextureDescriptor)!.FileName.Should().Be("foo.png");
        (textureList[2] as NoiseTextureDescriptor)!.Scale.Should().Be(1.5f);
    }

    [Test]
    public void CompoundMaterialList()
    {
        const string materialList =
            """
            [
                { "$type": "matte", "texture": "yellow" },
                { "$type": "metal", "fuzzy": 2.4, "texture": "blue" },
                { "$type": "glass", "ior": 1.9, "texture": "green" }
            ]
            """;

        var materials = JsonSerializer.Deserialize<List<IMaterialGenerator>>(materialList,
            YamlParser.JsonOptions);

        materials.Should().HaveCount(3);
        (materials[0] as MatteDescriptor)!.TextureReference.Should().Be("yellow");
        (materials[1] as MetalDescriptor)!.TextureReference.Should().Be("blue");
        (materials[1] as MetalDescriptor)!.Fuzzy.Should().Be(2.4f);
        (materials[2] as DielectricDescriptor)!.IOR.Should().Be(1.9f);
    }

    [Test]
    public void RenderOptionsParser()
    {
        const string renderOpts =
            """
            {   "numSamples": 500,
                "imageWidth": 320,
                "imageHeight": 200,
                "maxDepth": 50,
                "outputFile": "result.png"
            }
            """;

        var opts = JsonSerializer.Deserialize<RenderOptions>(renderOpts, YamlParser.JsonOptions);
        opts.NumSamples.Should().Be(500);
        opts.ImageWidth.Should().Be(320);
        opts.ImageHeight.Should().Be(200);
        opts.MaxDepth.Should().Be(50);
        opts.OutputFile.Should().Be("result.png");

        // partial defaults are preserved
        const string renderOpts2 = """
                                   {
                                      "numSamples": 500,
                                      "imageHeight": 200
                                   }
                                   """;

        opts = JsonSerializer.Deserialize<RenderOptions>(renderOpts2, YamlParser.JsonOptions);
        opts.NumSamples.Should().Be(500);
        opts.ImageWidth.Should().Be(800);
        opts.ImageHeight.Should().Be(200);
        opts.MaxDepth.Should().Be(50);
    }

    [Test]
    public void MeshFileParsing()
    {
        const string objFileScene =
            """
            {
                "fileName": "foo.obj",
                "normalize": true,
                "vertexNormals": true,
                "flipNormals": true
            }
            """;

        var mesh = JsonSerializer.Deserialize<MeshFileDescriptor>(objFileScene, YamlParser.JsonOptions);
        mesh.FileName.Should().Be("foo.obj");
        mesh.VertexNormals.Should().BeTrue();
        mesh.FlipNormals.Should().BeTrue();
        mesh.Normalize.Should().BeTrue();
    }

    [Test]
    public void QuadParsing()
    {
        const string quadJson = """
                                {
                                    "$type": "quad",
                                    "point": { "x": 2, "y": -2 },
                                    "axisU": { "x": 1 },
                                    "axisV": { "y": 1 },
                                    "material": "bla"
                                }
                                """;
        
        var quad = 
            JsonSerializer.Deserialize<IPrimitiveGenerator>(quadJson, YamlParser.JsonOptions);

        quad.Should().BeOfType<QuadDescriptor>();
        var qd = quad as QuadDescriptor;
        qd!.Point.Should().Be(new Vector3(2, -2, 0));
        qd.AxisU.Should().Be(new Vector3(1, 0, 0));
        qd.AxisV.Should().Be(new Vector3(0, 1, 0));
        qd.MaterialReference.Should().Be("bla");
    }
    
    [Test]
    public void SimpleSceneParsing()
    {
        var scene = JsonSerializer.Deserialize<SceneDescriptor>(SimpleSceneJson, YamlParser.JsonOptions);

        scene.Materials.Should().ContainKeys("blue", "green_metal");
        scene.Textures.Should().ContainKeys("blue", "greenish");
        scene.Materials["green_metal"].Should().BeOfType<MetalDescriptor>();
        var greenMetal = scene.Materials["green_metal"] as MetalDescriptor;
        greenMetal!.Fuzzy.Should().Be(0.5f);
        greenMetal.TextureReference.Should().Be("greenish");
    }
}