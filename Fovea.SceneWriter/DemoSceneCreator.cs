using System.Numerics;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.Viewing;

namespace Fovea.SceneWriter;

public static class DemoSceneCreator
{
    public static SceneDescriptor MakeScene(DemoSceneId sceneId)
    {
        return sceneId switch
        {
            DemoSceneId.FinalSceneBookOne => GetFinalSceneBookOne(false),
            DemoSceneId.FinalSceneBookOneMovingSpheres => GetFinalSceneBookOne(true),
            DemoSceneId.FinalSceneBookTwo => GetFinalSceneBookTwo(),
            _ => GetFinalSceneBookOne(false)
            // DemoScenes.ObjFileTest => GetObjFileTestScene(),
            // DemoScenes.CylinderTest => GetCylinderTestScene(),
            // DemoScenes.TextureDemo => GetTextureTestScene(),
            // DemoScenes.PerlinNoise => GetPerlinNoiseTestScene(),
            // DemoScenes.DiskTestScene => GetDiskTestScene(),
            // _ => GetCornellBoxScene()
        };
    }


    private static SceneDescriptor GetFinalSceneBookTwo()
    {
        var prims = new List<IPrimitiveGenerator>();

        var textures = new Dictionary<string, ITextureGenerator>
        {
            ["ground"] = new RGBColor(0.48f, 0.83f, 0.53f),
            ["diffLight"] = new RGBColor(7, 7, 7),
            ["moving"] = new RGBColor(0.7f, 0.3f, 0.1f),
            ["earth"] = new ImageTextureDescriptor { FileName = "assets/earth.jpg" },
            ["metal"] = new RGBColor(0.8f, 0.8f, 0.8f),
            ["cluster"] = new RGBColor(0.73f, 0.73f, 0.73f),
            ["noise"] = new NoiseTextureDescriptor { Scale = 0.1f },
            ["smokeA"] = new RGBColor(0.2f, 0.4f, 0.9f),
            ["white"] = RGBColor.White
        };

        var materials = new Dictionary<string, IMaterialGenerator>
        {
            ["ground"] = new MatteDescriptor { TextureReference = "ground" },
            ["diffLight"] = new DiffuseLightDescriptor { TextureReference = "diffLight" },
            ["glass"] = new DielectricDescriptor { IOR = 1.5f },
            ["earth"] = new MatteDescriptor { TextureReference = "earth" },
            ["metal"] = new MetalDescriptor { Fuzzy = 1, TextureReference = "metal" },
            ["moving"] = new MatteDescriptor { TextureReference = "moving" },
            ["cluster"] = new MatteDescriptor { TextureReference = "cluster" },
            ["noise"] = new MatteDescriptor { TextureReference = "noise" }
        };

        const int boxesPerSide = 20;
        for (var i = 0; i < boxesPerSide; ++i)
        for (var j = 0; j < boxesPerSide; ++j)
        {
            const float w = 100.0f;
            var x0 = -1000.0f + i * w;
            var z0 = -1000.0f + j * w;

            var x1 = x0 + w;
            var y1 = Sampler.Instance.Random(1, 101);
            var z1 = z0 + w;

            prims.Add(new BoxDescriptor
            {
                PointA = new Vector3(x0, 0, z0),
                PointB = new Vector3(x1, y1, z1),
                MaterialReference = "ground"
            });
        }

        prims.Add(new QuadDescriptor
        {
            MaterialReference = "diffLight",
            Point = new Vector3(123, 554, 147),
            AxisU = new Vector3(300, 0, 0),
            AxisV = new Vector3(0, 0, 265)
        });


        prims.AddRange([
            new SphereDescriptor
            {
                Center = new Vector3(400, 400, 400),
                Center1 = new Vector3(430, 400, 400),
                Radius = 50,
                MaterialReference = "moving"
            },
            new SphereDescriptor
            {
                Center = new Vector3(260, 150, 45),
                Radius = 50,
                MaterialReference = "glass"
            },
            new SphereDescriptor
            {
                Center = new Vector3(0, 150, 145),
                Radius = 50,
                MaterialReference = "metal"
            },
            new SphereDescriptor
            {
                Center = new Vector3(400, 200, 400),
                Radius = 100,
                MaterialReference = "earth"
            },
            new SphereDescriptor
            {
                Center = new Vector3(220, 280, 300),
                Radius = 80,
                MaterialReference = "noise"
            }
        ]);

        // medium inside glass...
        var boundaryShape = new SphereDescriptor
        {
            Center = new Vector3(360, 150, 145), Radius = 70,
            MaterialReference = "glass"
        };
        
        prims.Add(boundaryShape);
        prims.Add(new ConstantMediumDescriptor
        {
            Boundary = boundaryShape,
            Density = .02f,
            TextureReference = "smokeA"
        });

        var sceneSmokeBoundary = new SphereDescriptor
        {
            Center = Vector3.Zero,
            Radius = 5000.0f,
            MaterialReference = "glass"
        };
        prims.Add(new ConstantMediumDescriptor
        {
            Boundary = sceneSmokeBoundary,
            TextureReference = "white",
            Density = 0.0001f
        });
        
        // sphere cluster right top
        
        // var transform = new Transformation().Rotate(15, Axis.Y).Translate(-100, 270, 395);
        // var transform = new Transform().WithRotation(Axis.Y, 15).WithTranslation(-100, 270, 395);
        // var (sphereTransform, sphereInverse, _) = transform.Build();
        // prims.AddRange(Enumerable.Range(0, 1000).Select(_ =>
        // {
        //     var x = Sampler.Instance.RandomInt(0, 165);
        //     var y = Sampler.Instance.RandomInt(0, 165);
        //     var z = Sampler.Instance.RandomInt(0, 165);
        //
        //     return new Instance(
        //         new Sphere(new Vector3(x, y, z), 10, white),
        //         sphereTransform, sphereInverse);
        // }));
        
        // // moving sphere top left
        // var center1 = new Vector3(400, 400, 200);
        // prims.Add(new MovingSphere(center1, 0, center1 + new Vector3(30, 0, 0), 1, 50,
        //     new Lambertian(0.7f, 0.3f, 0.1f)));
        //
        // // glass sphere
        // prims.Add(new Sphere(new Vector3(260, 150, 45), 50, new Dielectric(1.5f)));
        // // metal sphere
        // prims.Add(new Sphere(new Vector3(0, 150, 145), 50, new Metal(0.8f, 0.8f, 0.9f, 1.0f)));
        //
        // // earth ball
        // prims.Add(new Sphere(new Vector3(400, 200, 400), 100,
        //     new Lambertian(new ImageTexture(@"Assets\earth.jpg"))));
        // // perlin noise ball
        // prims.Add(new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(new NoiseTexture(0.1f))));
        //
        // // isotropic material does not work as of now with the whole general path tracer
        //
        // // var boundary = new Sphere(new Point3(360, 150, 145), 70, new Dielectric(1.5));
        // // prims.Add(boundary);
        // // prims.Add(new ConstantMedium(boundary, 0.2, new RGBColor(0.2, 0.4, 0.9)));
        // // // some english fog to cover everything :)
        // // boundary = new Sphere(new Point3(), 5000, new Dielectric(1.5));
        // // prims.Add(new ConstantMedium(boundary, 0.0001, new RGBColor(1, 1, 1)));
        //
   
  
        return new SceneDescriptor
        {
            Primitives = prims,
            Textures = textures,
            Materials = materials,
            Background = RGBColor.Black,
            Camera = new CameraDescriptor
            {
                Orientation = new Orientation
                {
                    LookFrom = new Vector3(478, 278, -600),
                    LookAt = new Vector3(278, 278, 0),
                    UpDirection = Vector3.UnitY
                },
                FieldOfView = 40,
                DefocusAngle = 0
            },
            Options = new RenderOptions
            {
                ImageHeight = 600,
                ImageWidth = 800,
                MaxDepth = 50,
                NumSamples = 200
            }
        };
    }


    //
    // private static Scene GetDiskTestScene()
    // {
    //     var mat = new Lambertian(0.7f, 0.8f, 0.2f);
    //
    //     var prims = new List<IPrimitive>
    //     {
    //         new Sphere(new Vector3(0, -1000, 0), 999, new Lambertian(0.3f, 0.3f, 0.3f)),
    //         new Disk(new Vector3(-2, 2, 0), new Vector3(0, 0, 1), 1, mat),
    //         new Disk(new Vector3(0, 2, 0), new Vector3(0, 1, 1), 1, mat),
    //         new Disk(new Vector3(2, 2, 0), new Vector3(0, -1, 1), 1, mat),
    //
    //         new Disk(new Vector3(-2, 4, 0), new Vector3(0, 1, 0), 1, mat),
    //         new Disk(new Vector3(0, 4, 0), new Vector3(0, 1, -1), 1, mat),
    //         new Disk(new Vector3(2, 4, 0), new Vector3(0, 0, -1), 1, mat),
    //
    //         new Disk(new Vector3(-2, 0, 0), new Vector3(1, 1, 0), 1, mat),
    //         new Disk(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 1, mat),
    //         new Disk(new Vector3(2, 0, 0), new Vector3(-1, 1, 0), 1, mat)
    //     };
    //
    //     // Camera
    //     var orientation = new Orientation
    //     {
    //         LookFrom = new Vector3(0, 2, 4),
    //         LookAt = new Vector3(0, 2, 0),
    //         UpDirection = new Vector3(0, 1, 0)
    //     };
    //
    //     var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
    //     var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 70.0f, .1f, focusDist);
    //
    //     return new Scene
    //     {
    //         World = new BVHTree(prims),
    //         Camera = cam
    //     };
    // }
    //


    //
    // private static Scene GetCylinderTestScene()
    // {
    //     var prims = new List<IPrimitive>();
    //
    //     for (var a = 0; a < 360; a += 15)
    //     {
    //         var m = new Metal(Sampler.Instance.RandomColor(0.5f), Sampler.Instance.Random(0.0f, 0.05f));
    //         var cyl = new Cylinder(-1, 1, 0.3f, m);
    //         var tr = new Transform()
    //             .WithRotation(Axis.X, -90 + a)
    //             .WithTranslation(0, 5, -5)
    //             .WithRotation(Axis.Z, a);
    //             
    //         prims.Add(new Instance(cyl, tr));
    //     }
    //
    //     // Camera
    //     var orientation = new Orientation
    //     {
    //         LookFrom = new Vector3(-3, 0, 4),
    //         LookAt = new Vector3(-2, 0, 0),
    //         UpDirection = new Vector3(0, 1, 0)
    //     };
    //
    //     var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
    //     var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 75.0f, .1f, focusDist);
    //
    //     return new Scene
    //     {
    //         World = new BVHTree(prims),
    //         Camera = cam
    //     };
    // }
    //


    private static SceneDescriptor GetFinalSceneBookOne(bool withMovingSpheres)
    {
        var textures = new Dictionary<string, ITextureGenerator>()
        {
            ["ground"] = new RGBColor(0.5f, 0.5f, 0.5f),
            ["leftSphere"] = new RGBColor(0.4f, 0.2f, 0.1f),
            ["rightSphere"] = new RGBColor(0.7f, 0.6f, 0.5f)
        };

        var materials = new Dictionary<string, IMaterialGenerator>
        {
            ["ground"] = new MatteDescriptor { TextureReference = "ground" },
            ["leftSphere"] = new MatteDescriptor { TextureReference = "leftSphere" },
            ["rightSphere"] = new MetalDescriptor { TextureReference = "rightSphere" },
            ["glass"] = new DielectricDescriptor { IOR = 1.5f }
        };

        var prims = new List<IPrimitiveGenerator>
        {
            new SphereDescriptor { Center = new Vector3(0, -1000, 0), Radius = 1000, MaterialReference = "ground" },
            new SphereDescriptor { Center = new Vector3(0, 1, 0), Radius = 1, MaterialReference = "glass" },
            new SphereDescriptor { Center = new Vector3(-4, 1, 0), Radius = 1, MaterialReference = "leftSphere" },
            new SphereDescriptor { Center = new Vector3(4, 1, 0), Radius = 1, MaterialReference = "rightSphere" },
        };

        // generate random positions which are around the three main spheres
        var offLimitsZone = new Vector3(4, 0.2f, 0);
        var sphereCenters = Enumerable
            .Range(-11, 22)
            .SelectMany(a => Enumerable.Range(-11, 22).Select(b => (a, b)))
            .Select(tpl =>
                new Vector3(
                    tpl.a + 0.9f * Sampler.Instance.Random01(),
                    0.2f,
                    tpl.b + 0.9f * Sampler.Instance.Random01()))
            .Where(center => (center - offLimitsZone).Length() > 0.9f)
            .ToList();

        // need unique names
        var materialSuffix = 0;
        var textureSuffix = 0;
        foreach (var center in sphereCenters)
        {
            var materialRnd = Sampler.Instance.Random01();
            var materialName = $"mat{materialSuffix++}";
            if (materialRnd < 0.8f) // matte
            {
                var textureName = $"tex{textureSuffix++}";
                textures[textureName] = Sampler.Instance.RandomColor() * Sampler.Instance.RandomColor();
                materials.Add(materialName, new MatteDescriptor { TextureReference = textureName });

                if (withMovingSpheres)
                {
                    var center1 = center + new Vector3(0, Sampler.Instance.Random(0, 0.5f), 0);
                    prims.Add(new SphereDescriptor
                    {
                        Center = center,
                        Center1 = center1,
                        MaterialReference = materialName,
                        Radius = 0.2f
                    });
                    continue;
                }
            }
            else if (materialRnd < 0.95f) // metal
            {
                var textureName = $"tex{textureSuffix++}";
                textures[textureName] = Sampler.Instance.RandomColor(0.5f);
                materials.Add(materialName,
                    new MetalDescriptor { Fuzzy = Sampler.Instance.Random(0, 0.05f), TextureReference = textureName });
            }
            else // glass
            {
                materials.Add(materialName, new DielectricDescriptor { IOR = 1.5f });
            }

            prims.Add(new SphereDescriptor
            {
                Center = center,
                MaterialReference = materialName,
                Radius = 0.2f
            });
        }

        var orientation = new Orientation
        {
            LookFrom = new Vector3(13, 2, 3),
            LookAt = new Vector3(0, 0, 0),
            UpDirection = new Vector3(0, 1, 0)
        };

        return new SceneDescriptor
        {
            Camera = new CameraDescriptor
            {
                FocusDistance = 10.0f,
                DefocusAngle = 0.6f,
                FieldOfView = 20.0f,
                Orientation = orientation
            },
            Options = new RenderOptions
            {
                ImageHeight = 600,
                ImageWidth = 800,
                MaxDepth = 50,
                NumSamples = 200
            },
            Materials = materials,
            Primitives = prims,
            Textures = textures
        };
    }
}