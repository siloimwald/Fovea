using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Image;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.Viewing;

namespace Fovea.CmdLine
{
    public enum DemoScenes
    {
        FinalSceneBookOne,
        FinalSceneBookTwo,
        ObjFileTest,
        CylinderTest,
        TextureDemo,
        PerlinNoise,
        DiskTestScene,
        CornellBox
    }

    public static class DemoSceneCreator
    {
        private const float DefaultAspectRatio = 16.0f / 9.0f;

        public static Scene MakeScene(DemoScenes sceneId, int imageWidth)
        {
            var scene = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => GetFinalSceneBookOne(),
                DemoScenes.ObjFileTest => GetObjFileTestScene(),
                DemoScenes.CylinderTest => GetCylinderTestScene(),
                DemoScenes.TextureDemo => GetTextureTestScene(),
                DemoScenes.PerlinNoise => GetPerlinNoiseTestScene(),
                DemoScenes.DiskTestScene => GetDiskTestScene(),
                DemoScenes.FinalSceneBookTwo => GetFinalSceneBookTwo(),
                _ => GetCornellBoxScene()
            };

            scene.OutputSize = (imageWidth, (int) (imageWidth / scene.Camera.AspectRatio));
            return scene;
        }

        private static Scene GetFinalSceneBookTwo()
        {
            var prims = new List<IPrimitive>();
            var baseBoxMaterial = new Lambertian(0.48f, 0.83f, 0.53f);

            const int boxesPerSide = 20;
            for (var i = 0; i < boxesPerSide; ++i)
            for (var j = 0; j < boxesPerSide; ++j)
            {
                const float w = 100.0f;
                var x0 = -1000.0f + i * w;
                var z0 = -1000.0f + j * w;
                var y1 = (float)Sampler.Instance.Random(1, 101);

                prims.AddRange(BoxProducer.Produce(x0, x0 + w, 0.0f, y1, z0, z0 + w)
                    .CreateSingleTriangles(baseBoxMaterial));
            }

            // light source
            var lightSource = new List<IPrimitive>();
            lightSource.AddRange(QuadProducer.Produce(123, 412, 147, 423, 554, Axis.Y)
                .CreateMeshTriangles(new DiffuseLight(new RGBColor(7, 7, 7)), true));

            prims.AddRange(lightSource);

            // moving sphere top left
            var center1 = new Vector3(400, 400, 200);
            prims.Add(new MovingSphere(center1, 0, center1 + new Vector3(30, 0, 0), 1, 50,
                new Lambertian(0.7f, 0.3f, 0.1f)));

            // glass sphere
            prims.Add(new Sphere(new Vector3(260, 150, 45), 50, new Dielectric(1.5f)));
            // metal sphere
            prims.Add(new Sphere(new Vector3(0, 150, 145), 50, new Metal(0.8f, 0.8f, 0.9f, 1.0f)));

            // earth ball
            prims.Add(new Sphere(new Vector3(400, 200, 400), 100,
                new Lambertian(new ImageTexture(@"Assets\earth.jpg"))));
            // perlin noise ball
            prims.Add(new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(new NoiseTexture(0.1f))));

            // isotropic material does not work as of now with the whole general path tracer

            // var boundary = new Sphere(new Point3(360, 150, 145), 70, new Dielectric(1.5));
            // prims.Add(boundary);
            // prims.Add(new ConstantMedium(boundary, 0.2, new RGBColor(0.2, 0.4, 0.9)));
            // // some english fog to cover everything :)
            // boundary = new Sphere(new Point3(), 5000, new Dielectric(1.5));
            // prims.Add(new ConstantMedium(boundary, 0.0001, new RGBColor(1, 1, 1)));

            // sphere cluster right top
            var white = new Lambertian(0.73f, 0.73f, 0.73f);
            // var transform = new Transformation().Rotate(15, Axis.Y).Translate(-100, 270, 395);
            var transform = new Transform().WithRotation(Axis.Y, 15).WithTranslation(-100, 270, 395);
            var (sphereTransform, sphereInverse, _) = transform.Build();
            prims.AddRange(Enumerable.Range(0, 1000).Select(_ =>
            {
                var x = Sampler.Instance.RandomInt(0, 165);
                var y = Sampler.Instance.RandomInt(0, 165);
                var z = Sampler.Instance.RandomInt(0, 165);

                return new Instance(
                    new Sphere(new Vector3(x, y, z), 10, white),
                    sphereTransform, sphereInverse);
            }));

            var orientation = new Orientation
            {
                LookFrom = new Vector3(478, 278, -600),
                LookAt = new Vector3(278, 278, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var cam = new PerspectiveCamera(orientation, 1, 40.0f, 0, 10.0f, 0, 1);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam,
                Background = new RGBColor(),
                Lights = new PrimitiveList(lightSource)
            };
        }

        private static Scene GetCornellBoxScene()
        {
            var red = new Lambertian(0.65f, 0.05f, 0.05f);
            var white = new Lambertian(0.73f, 0.73f, 0.73f);
            var green = new Lambertian(0.12f, 0.45f, 0.15f);
            var light = new DiffuseLight(new RGBColor(5, 5, 5));

            var prims = new List<IPrimitive>();

            // rectangular light source
            // var lightSource =
            //     new PrimitiveList(QuadProducer.Produce(213, 343, 227, 332, 554, Axis.Y)
            //         .CreateMeshTriangles(light, flipNormals:true)); 
            // prims.Add(lightSource);

            // spot lights
            var spotLights = new List<IPrimitive>
            {
                new Disk(new Vector3(140, 554, 140), new Vector3(0, -1, 0), 40, light),
                new Disk(new Vector3(415, 554, 140), new Vector3(0, -1, 0), 40, light),
                // new Disk(new Vector3(140, 554, 415), new Vec3(0, -1, 0), 40, light),
                // new Disk(new Vector3(415, 554, 415), new Vec3(0, -1, 0), 40, light),
                new Disk(new Vector3(1, 415, 278), new Vector3(1, 0, 0), 40, light),
                new Disk(new Vector3(554, 415, 278), new Vector3(-1, 0, 0), 40, light)
            };
            prims.AddRange(spotLights);
            var lightSources = new PrimitiveList(spotLights);

            // wall in the back
            prims.AddRange(QuadProducer.Produce(0, 555, 0, 555, 555, Axis.Z).CreateSingleTriangles(white));
            // floor and ceiling
            prims.AddRange(QuadProducer.Produce(0, 555, 0, 555, 555, Axis.Y).CreateSingleTriangles(white));
            prims.AddRange(QuadProducer.Produce(0, 555, 0, 555, 0, Axis.Y).CreateSingleTriangles(white));
            // left and right wall
            prims.AddRange(QuadProducer.Produce(0, 555, 0, 555, 0, Axis.X).CreateSingleTriangles(red));
            prims.AddRange(QuadProducer.Produce(0, 555, 0, 555, 555, Axis.X).CreateSingleTriangles(green));

            // tall left box
            // don't use instancing here, rather transform stuff directly
            var leftBox = BoxProducer.Produce(0, 165, 0, 330, 0, 165)
                .ApplyTransform(new Transform().WithRotation(Axis.Y, 15).WithTranslation(265, 0, 295).BuildForwardOnly())
                // .CreateSingleTriangles(new Metal(0.8, 0.85, 0.88, 0));
                .CreateSingleTriangles(white);
            // var leftBoxSmoke = new ConstantMedium(new PrimitiveList(leftBox), 0.01, new RGBColor(0, 0, 0));
            prims.AddRange(leftBox);

            // var rightBox = BoxProducer.Produce(0, 165, 0, 165, 0, 165)
            //     .ApplyTransform(new Transformation().Rotate(-18, Axis.Y).Translate(130, 0, 65).GetMatrix())
            //     .CreateSingleTriangles(white);
            // prims.AddRange(rightBox);

            var glassSphere = new Sphere(new Vector3(190, 90, 190), 90, new Dielectric(1.5f));
            prims.Add(glassSphere);
            lightSources.Add(glassSphere);

            var orientation = new Orientation
            {
                LookFrom = new Vector3(278, 278, -800),
                LookAt = new Vector3(278, 278, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var cam = new PerspectiveCamera(orientation, 1.0f, 40.0f, 0, 10.0f);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam,
                Background = new RGBColor(),
                Lights = lightSources
            };
        }

        private static Scene GetDiskTestScene()
        {
            var mat = new Lambertian(0.7f, 0.8f, 0.2f);

            var prims = new List<IPrimitive>
            {
                new Sphere(new Vector3(0, -1000, 0), 999, new Lambertian(0.3f, 0.3f, 0.3f)),
                new Disk(new Vector3(-2, 2, 0), new Vector3(0, 0, 1), 1, mat),
                new Disk(new Vector3(0, 2, 0), new Vector3(0, 1, 1), 1, mat),
                new Disk(new Vector3(2, 2, 0), new Vector3(0, -1, 1), 1, mat),

                new Disk(new Vector3(-2, 4, 0), new Vector3(0, 1, 0), 1, mat),
                new Disk(new Vector3(0, 4, 0), new Vector3(0, 1, -1), 1, mat),
                new Disk(new Vector3(2, 4, 0), new Vector3(0, 0, -1), 1, mat),

                new Disk(new Vector3(-2, 0, 0), new Vector3(1, 1, 0), 1, mat),
                new Disk(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 1, mat),
                new Disk(new Vector3(2, 0, 0), new Vector3(-1, 1, 0), 1, mat)
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Vector3(0, 2, 4),
                LookAt = new Vector3(0, 2, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 70.0f, .1f, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        private static Scene GetPerlinNoiseTestScene()
        {
            var perlinTex
                = new Lambertian(new NoiseTexture(4));

            var prims = new List<IPrimitive>
            {
                new Sphere(new Vector3(0, -1000, 0), 1000, perlinTex),
                new Sphere(new Vector3(0, 2, 0), 2, perlinTex)
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Vector3(13, 2, 3),
                LookAt = new Vector3(0, 0, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1f, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetTextureTestScene()
        {
            var checker2 = new Lambertian(new CheckerBoard(new RGBColor(0.2f, 0.3f, 0.3f), new RGBColor(0.9f)));
            var earth = new Lambertian(new ImageTexture(@"Assets\earth.jpg"));
            var baseCylinder = new Cylinder(0, 4, 1, new Lambertian(new NoiseTexture(4)));
            var prims = new List<IPrimitive>
            {
                new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(0.6f, 0.5f, 0.3f)),
                new Sphere(new Vector3(3, 2, -1.5f), 2, earth),
                new Sphere(new Vector3(1.5f, 1, 2), 1, checker2),
                new Instance(baseCylinder, new Transform().WithRotation(Axis.X, -90).WithTranslation(-1, 0, 0)),
                new Instance(baseCylinder, new Transform().WithRotation(Axis.Y, 45).WithTranslation(-4.5f, 1, 0))
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Vector3(1, 5, 5),
                LookAt = new Vector3(0, 1, 0),
                UpDirection = new Vector3(0, 0, -1)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 75.0f, .1f, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetCylinderTestScene()
        {
            var prims = new List<IPrimitive>();

            for (var a = 0; a < 360; a += 15)
            {
                var m = new Metal(Sampler.Instance.RandomColor(0.5f), Sampler.Instance.Random(0.0f, 0.05f));
                var cyl = new Cylinder(-1, 1, 0.3f, m);
                var tr = new Transform()
                    .WithRotation(Axis.X, -90 + a)
                    .WithTranslation(0, 5, -5)
                    .WithRotation(Axis.Z, a);
                
                prims.Add(new Instance(cyl, tr));
            }

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Vector3(-3, 0, 4),
                LookAt = new Vector3(-2, 0, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 75.0f, .1f, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        // read teapot.obj and render it
        private static Scene GetObjFileTestScene()
        {
            var groundMaterial = new Lambertian(new ImageTexture(@"Assets\cb.jpg"));
            var metallic = new Metal(0.8f, 0.85f, 0.88f, 0.3f);
            var prims = new List<IPrimitive>();

            prims.AddRange(QuadProducer.Produce(-1, 1, -1, 1, -.4f, Axis.Y)
                .CreateMeshTriangles(groundMaterial, true));

            var mesh = ObjReader.ReadObjFile(@"assets\teapot.obj", true);

            var env = new FlipFace(
                new Sphere(new Vector3(), 10, new Lambertian(new ImageTexture(@"Assets\forest.jpg"))));

            // var diffuseLight = new DiffuseLight(new RGBColor(5, 5, 6));
            // var lightSource = new PrimitiveList(QuadProducer.Produce(-3, 3, -3, 3, 5, Axis.Y)
            //     .CreateMeshTriangles(diffuseLight, true));
            // prims.Add(lightSource);
            prims.AddRange(mesh.CreateMeshTriangles(metallic, false, true));

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Vector3(1, 0.5f, -3),
                LookAt = new Vector3(0, 0, 0),
                UpDirection = new Vector3(0, 1, 0)
            };
            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 28.0f, .1f, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam,
                Background = new RGBColor(),
                // Lights = lightSource,
                Environment = env
            };
        }

        private static Scene GetFinalSceneBookOne()
        {
            var prims = new List<IPrimitive>();
            var groundMat = new Lambertian(0.5f, 0.5f, 0.5f);
            prims.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groundMat));
            prims.Add(new Sphere(new Vector3(0, 1, 0), 1, new Dielectric(1.5f)));
            prims.Add(new Sphere(new Vector3(-4, 1, 0), 1, new Lambertian(0.4f, 0.2f, 0.1f)));
            prims.Add(new Sphere(new Vector3(4, 1, 0), 1, new Metal(0.7f, 0.6f, 0.5f)));

            // just for the sake of writing some slightly more modern c#

            IMaterial RandomMaterial()
            {
                var r = Sampler.Instance.Random01();
                return r switch
                {
                    < 0.8f => new Lambertian(Sampler.Instance.RandomColor() * Sampler.Instance.RandomColor()),
                    < 0.95f => new Metal(Sampler.Instance.RandomColor(0.5f), Sampler.Instance.Random(0.0f, 0.05f)),
                    _ => new Dielectric(1.5f)
                };
            }

            var offLimitsZone = new Vector3(4, 0.2f, 0);

            prims.AddRange(Enumerable
                .Range(-11, 22)
                .SelectMany(a => Enumerable.Range(-11, 22).Select(b => (a, b)))
                .Select(tpl =>
                    new Vector3(
                        tpl.a + 0.9f * (float)Sampler.Instance.Random01(),
                        0.2f,
                        tpl.b + 0.9f * (float)Sampler.Instance.Random01()))
                .Where(center => (center - offLimitsZone).Length() > 0.9f)
                .Select<Vector3, IPrimitive>(center =>
                {
                    var mat = RandomMaterial();
                    if (mat is not Lambertian) return new Sphere(center, 0.2f, RandomMaterial());
                    var center2 = center + new Vector3(0, (float)Sampler.Instance.Random(0, 0.5f), 0);
                    return new MovingSphere(center, 0, center2, 1, 0.2f, mat);
                }));

            var orientation = new Orientation
            {
                LookFrom = new Vector3(13, 2, 3),
                LookAt = new Vector3(0, 0, 0),
                UpDirection = new Vector3(0, 1, 0)
            };

            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1f, 10.0f, 0, 1);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }
    }
}