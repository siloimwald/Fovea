using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using FluentAssertions;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Parser.Json;
using Fovea.Renderer.VectorMath;
using NUnit.Framework;
using SixLabors.ImageSharp.Formats.Png;

namespace Fovea.Tests;

public class JsonTests
{
    private const string SimpleSceneJson = """
                                           {
                                            "options": {
                                                "samples": 250
                                            },
                                           "background": { "r": 2, "b": 0.4 },
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

        var sphereDescriptor = JsonSerializer
            .Deserialize<SphereDescriptor>(sphereJson,
                JsonParser.JsonOptions);

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
            JsonParser.JsonOptions);

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
            JsonParser.JsonOptions);

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
            JsonParser.JsonOptions);

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

        var opts = JsonSerializer.Deserialize<RenderOptions>(renderOpts, JsonParser.JsonOptions);
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

        opts = JsonSerializer.Deserialize<RenderOptions>(renderOpts2, JsonParser.JsonOptions);
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
                "$type": "meshFile",
                "fileName": "foo.obj",
                "normalize": true,
                "vertexNormals": true,
                "flipNormals": true
            }
            """;

        var meshPrim = JsonSerializer.Deserialize<IPrimitiveGenerator>(objFileScene, JsonParser.JsonOptions);
        meshPrim.Should().BeOfType<MeshFileDescriptor>();
        var mesh = meshPrim as MeshFileDescriptor;
        mesh!.FileName.Should().Be("foo.obj");
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
            JsonSerializer.Deserialize<IPrimitiveGenerator>(quadJson, JsonParser.JsonOptions);

        quad.Should().BeOfType<QuadDescriptor>();
        var qd = quad as QuadDescriptor;
        qd!.Point.Should().Be(new Vector3(2, -2, 0));
        qd.AxisU.Should().Be(new Vector3(1, 0, 0));
        qd.AxisV.Should().Be(new Vector3(0, 1, 0));
        qd.MaterialReference.Should().Be("bla");
    }
    
    [Test]
    public void BoxParsing()
    {
        const string boxJson = """
                                {
                                    "$type": "box",
                                    "pointA": { "x": 2, "y": -2 },
                                    "pointB": { "x": 1, "y": -4, "z": 1},
                                    "material": "blupp"
                                }
                                """;
        
        var box = 
            JsonSerializer.Deserialize<IPrimitiveGenerator>(boxJson, JsonParser.JsonOptions);

        box.Should().BeOfType<BoxDescriptor>();
        var qd = box as BoxDescriptor;
        qd!.PointA.Should().Be(new Vector3(2, -2, 0));
        qd.PointB.Should().Be(new Vector3(1, -4, 1));
        qd.MaterialReference.Should().Be("blupp");
    }
    
    [Test]
    public void SimpleSceneParsing()
    {
        var scene = JsonSerializer.Deserialize<SceneDescriptor>(SimpleSceneJson, JsonParser.JsonOptions);

        scene.Background.Should().Be(new RGBColor(2, 0, 0.4f));
        scene.Materials.Should().ContainKeys("blue", "green_metal");
        scene.Textures.Should().ContainKeys("blue", "greenish");
        scene.Materials["green_metal"].Should().BeOfType<MetalDescriptor>();
        var greenMetal = scene.Materials["green_metal"] as MetalDescriptor;
        greenMetal!.Fuzzy.Should().Be(0.5f);
        greenMetal.TextureReference.Should().Be("greenish");
    }
    
    [Test]
    public void DiffuseLightParsing()
    {
        const string diffLightJson = """
                                     {
                                        "$type": "diffuseLight",
                                        "texture": "blurp"  
                                     }
                                     """;
        var diffLight = JsonSerializer.Deserialize<IMaterialGenerator>(diffLightJson, JsonParser.JsonOptions);
        diffLight.Should().BeOfType<DiffuseLightDescriptor>();
        (diffLight as DiffuseLightDescriptor)!.TextureReference.Should().Be("blurp");
    }

    [Test]
    public void TransformsParsing()
    {
        const string transforms = """
                                  [
                                  { "$type": "translate", "x": 5, "y": -1 },
                                  { "$type": "scale", "x": 2, "y": 2, "z": 2 },
                                  { "$type": "rotate", "axis": "z", "by": 90.0 }
                                  ]
                                  """;

        var transformList = JsonSerializer
            .Deserialize<List<ITransformationDescriptor>>(transforms, JsonParser.JsonOptions);

        transformList.Should().HaveCount(3);
        transformList[0].Should().BeOfType<TranslationDescriptor>();
        transformList[1].Should().BeOfType<ScalingDescriptor>();
        transformList[2].Should().BeOfType<RotationDescriptor>();

        (transformList[0] as TranslationDescriptor)!.X.Should().Be(5);
        (transformList[1] as ScalingDescriptor)!.Y.Should().Be(2);
        (transformList[2] as RotationDescriptor)!.Axis.Should().Be(Axis.Z);
        (transformList[2] as RotationDescriptor)!.Angle.Should().Be(90);
        
        var forwardMatrix = transformList.GetTransformation();
        var p = new Vector3(-2, 1, 0);
        var pTransformed = Vector3.Transform(p, forwardMatrix);
        var dist = (new Vector3(0, 6, 0) - pTransformed).LengthSquared();
        dist.Should().BeLessThan(1e-5f);
    }

    [Test]
    public void InstanceParsing()
    {
        const string instanceJson = """
                                    { "$type": "instance",
                                       "blueprint": "shoe",
                                       "material": "red",
                                       "transforms": 
                                               [
                                            { "$type": "translate", "x": 5, "y": -1 },
                                            { "$type": "scale", "x": 2, "y": 2, "z": 2 },
                                            { "$type": "rotate", "axis": "z", "by": 90.0 }
                                            ]
                                    }
                                    """;

        var instanceDescriptor = JsonSerializer.Deserialize<IPrimitiveGenerator>(
            instanceJson, JsonParser.JsonOptions);

        instanceDescriptor.Should().BeOfType<InstanceDescriptor>();
        var instance = instanceDescriptor as InstanceDescriptor;
        instance!.BlueprintName.Should().Be("shoe");
        instance!.MaterialReference.Should().Be("red");
        instance.Transforms.Should().HaveCount(3);
    }
    
}