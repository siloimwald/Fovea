using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Image;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;
using Fovea.Renderer.Mesh;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.Primitives.CSG;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.VectorMath.Transforms;
using Fovea.Renderer.Viewing;

namespace Fovea.CmdLine
{
    public enum DemoScenes
    {
        FinalSceneBookOne,
        SphereCSGTest,
        BoxCSGTest,
        BoxTest,
        ObjFileTest,
        CylinderTest,
        HollowGlass,
        TextureDemo,
        PerlinNoise
    }

    public static class DemoSceneCreator
    {
        private const double DefaultAspectRatio = 16.0 / 9.0;

        public static Scene MakeScene(DemoScenes sceneId, int imageWidth)
        {
            var scene = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => GetFinalSceneBookOne(),
                DemoScenes.SphereCSGTest => GetSphereCSGTestScene(),
                DemoScenes.BoxTest => GetBoxTestScene(),
                DemoScenes.BoxCSGTest => GetBoxCSGTestScene(),
                DemoScenes.ObjFileTest => GetObjFileTestScene(),
                DemoScenes.CylinderTest => GetCylinderTestScene(),
                DemoScenes.TextureDemo => GetTextureTestScene(),
                DemoScenes.PerlinNoise => GetPerlinNoiseTestScene(),
                _ => GetHollowGlassScene()
            };

            scene.OutputSize = (imageWidth, (int) (imageWidth / DefaultAspectRatio));
            return scene;
        }

        private static Scene GetPerlinNoiseTestScene()
        {
            var perlinTex
                = new Lambertian(new NoiseTexture(4));
            
            var prims = new List<IPrimitive>
            {
                new Sphere(new Point3(0, -1000, 0), 1000, perlinTex),
                new Sphere(new Point3(0,2,0), 2, perlinTex)
            };
            
            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(13, 2, 3),
                LookAt = new Point3(0, 0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetTextureTestScene()
        {
            var checker2 = new Lambertian(new CheckerBoard(new RGBColor(0.2, 0.3, 0.3), new RGBColor(0.9)));
            //var checker2 = new Lambertian(new ImageTexture(@"Assets\cb.jpg"));
            var earth = new Lambertian(new ImageTexture(@"Assets\earth.jpg"));
            var baseCylinder = new Cylinder(0, 4, 1, checker2);
            var prims = new List<IPrimitive>
            {
                new Sphere(new Point3(0, -1000, 0), 1000, new Lambertian(0.6, 0.5, 0.3)),
                new Sphere(new Point3(3, 2, -1.5), 2, earth),
                new Sphere(new Point3(1.5, 1, 2), 1, checker2),
                new Instance(baseCylinder, new Transformation().Rotate(-90, Axis.X).Translate(-1,0,0)),
                new Instance(baseCylinder, new Transformation().Rotate(45, Axis.Y).Translate(-4.5,1,0))
            };
            
            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(1, 5, 5),
                LookAt = new Point3(0, 1, 0),
                UpDirection = new Vec3(0, 0, -1)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 75.0f, .1, focusDist);

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
                var m = new Metal(Sampler.Instance.RandomColor(0.5f, 1.0f), Sampler.Instance.Random(0.0, 0.05));
                var cyl = new Cylinder(-1, 1, 0.3, m);
                var tr = new Transformation()
                    .Rotate(-90 + a, Axis.X)
                    .Translate(0, 5, -5)
                    .Rotate(a, Axis.Z);
                prims.Add(new Instance(cyl, tr.GetMatrix(), tr.GetInverseMatrix()));
            }

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(-3, 0, 4),
                LookAt = new Point3(-2, 0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 75.0f, .1, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        // read teapot.obj and render it
        private static Scene GetObjFileTestScene()
        {
            var groundMaterial = new Lambertian(0.5, 0.5, 0.5);

            var groundPlane0 = new Triangle(new Point3(-5, -1, 5), new Point3(5, -1, 5),
                new Point3(5, -1, -5),
                groundMaterial);

            var groundPlane1 = new Triangle(new Point3(5, -1, -5), new Point3(-5, -1, -5),
                new Point3(-5, -1, 5), groundMaterial);

            var yellowish = new Lambertian(0.8, 0.7, 0.1);

            var prims = new List<IPrimitive>
            {
                groundPlane0,
                groundPlane1
            };

            var mesh = ObjReader.ReadObjFile(@"assets\teapot.obj", true);

            prims.AddRange(mesh.CreateSingleTriangles(yellowish));

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(-3, 3, 2),
                LookAt = new Point3(0, 0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        /// <summary>
        /// box 'wireframe' made by cutting out the inner parts through csg
        /// </summary>
        /// <returns></returns>
        private static Scene GetBoxCSGTestScene()
        {
            var groundMaterial = new Lambertian(0.5, 0.5, 0.5);

            var groundPlane0 = new Triangle(new Point3(-100, -3, 100), new Point3(100, -3, 100),
                new Point3(100, -3, -100),
                groundMaterial);

            var groundPlane1 = new Triangle(new Point3(100, -3, -100), new Point3(-100, -3, -100),
                new Point3(-100, -3, 100), groundMaterial);

            var greenish = new Lambertian(0.2, 0.8, 0.3);

            var outerBox = BoxProducer.Produce(-1, 1, -1, 1, -1, 1).CreateSingleTriangles(greenish);
            // cut front to back
            var cut0 = new PrimitiveList(
                BoxProducer.Produce(-0.8, 0.8, -0.8, 0.8, -1.2, 1.2).CreateSingleTriangles(greenish));
            // cut left to right
            var cut1 =
                new PrimitiveList(
                    BoxProducer.Produce(-1.2, 1.2, -0.8, 0.8, -0.8, 0.8).CreateSingleTriangles(greenish));
            // cut top to bottom
            var cut2 = new PrimitiveList(
                BoxProducer.Produce(-0.8, 0.8, -1.2, 1.2, -0.8, 0.8).CreateSingleTriangles(greenish));

            var redSphere = new Sphere(new Point3(0, 0, 0), 1, new Lambertian(0.8, 0.2, 0.1));
            var sphereCutOut = new Sphere(new Point3(0.5, 0, 0), 0.8, greenish);
            var sphereCsg =
                new CSGPrimitive(redSphere, sphereCutOut, CSGOperation.Difference);

            var csg =
                new CSGPrimitive(new PrimitiveList(outerBox),
                    new CSGPrimitive(cut2, new CSGPrimitive(cut0,
                        cut1, CSGOperation.Union), CSGOperation.Union),
                    CSGOperation.Difference);

            var prims = new List<IPrimitive>
            {
                groundPlane0,
                groundPlane1,
                csg,
                sphereCsg
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(8, 4, 3),
                LookAt = new Point3(0, 0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        /// a glass box in front of a metal box with a sphere between them
        private static Scene GetBoxTestScene()
        {
            var groundMaterial = new Lambertian(0.8, 0.8, 0.0);

            var groundPlane0 = new Triangle(new Point3(-100, -1, 100), new Point3(100, -1, 100),
                new Point3(100, -1, -100),
                groundMaterial);

            var groundPlane1 = new Triangle(new Point3(100, -1, -100), new Point3(-100, -1, -100),
                new Point3(-100, -1, 100), groundMaterial);

            var blueBall = new Sphere(new Point3(1, 0.5, -7.5), 0.5, new Lambertian(0.1, 0.2, 0.6));
            var glassBox = BoxProducer.Produce(-2, 2, -1, 2, -5.5, -6).CreateSingleTriangles(new Dielectric(5.5));
            var metalBox = BoxProducer.Produce(-4, 3, -1, 6, -8, -9).CreateSingleTriangles(new Metal(0.7, 0.5, 0.2, 0));

            var prims = new List<IPrimitive>
            {
                blueBall,
                groundPlane0,
                groundPlane1
            };

            prims.AddRange(metalBox);
            prims.AddRange(glassBox);

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(12, 10.5, 3),
                LookAt = new Point3(1, 0.5, -7),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0f, .1, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        private static Scene GetSphereCSGTestScene()
        {
            var blueish = new Lambertian(0.1, 0.2, 0.5);
            var greenish = new Lambertian(0.1, 0.6, 0.2);
            var groundMaterial = new Lambertian(0.8, 0.8, 0.0);

            var centerSphere = new Sphere(new Point3(0, 0, -1), 1, blueish);

            var cutFront = new Sphere(new Point3(0, 0.0, -0.2), 0.6, greenish);
            var cutRight = new Sphere(new Point3(0.8, 0.0, -1), 0.6, greenish);
            var cutLeft = new Sphere(new Point3(-0.8, 0.0, -1), 0.6, greenish);
            var cutBack = new Sphere(new Point3(0, 0, -1.8), 0.6, greenish);

            var cut =
                new CSGPrimitive(cutBack,
                    new CSGPrimitive(cutLeft,
                        new CSGPrimitive(cutFront, cutRight, CSGOperation.Union), CSGOperation.Union)
                    , CSGOperation.Union);

            var blob = new CSGPrimitive(centerSphere, cut, CSGOperation.Difference);

            var prims = new List<IPrimitive>
            {
                blob,
                new Sphere(new Point3(0, -101, -1), 100, groundMaterial),
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(2, 1, 2),
                LookAt = new Point3(0, 0, -1),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 60.0f, .1, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetHollowGlassScene()
        {
            var centerMaterial = new Lambertian(0.1, 0.2, 0.5);
            var groundMaterial = new Lambertian(0.8, 0.8, 0.0);
            var materialLeft = new Dielectric(1.5);
            var materialRight = new Metal(0.8, 0.6, 0.2, 0.1);
            var prims = new List<IPrimitive>
            {
                new Sphere(new Point3(0, 0, -1), 0.5, centerMaterial),
                new Sphere(new Point3(0, -100.5, -1), 100, groundMaterial),
                new Sphere(new Point3(-1, 0, -1), 0.5, materialLeft),
                new Sphere(new Point3(-1, 0, -1), -0.45, materialLeft),
                new Sphere(new Point3(1, 0, -1), 0.5, materialRight)
            };

            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(0, 0, 0),
                LookAt = new Point3(0, 0, -1),
                UpDirection = new Vec3(0, 1, 0)
            };

            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 90.0f, .1, focusDist);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }

        private static Scene GetFinalSceneBookOne()
        {
            var prims = new List<IPrimitive>();
            var groundMat = new Lambertian(0.5, 0.5, 0.5);
            prims.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMat));
            prims.Add(new Sphere(new Point3(0, 1, 0), 1, new Dielectric(1.5)));
            prims.Add(new Sphere(new Point3(-4, 1, 0), 1, new Lambertian(0.4, 0.2, 0.1)));
            prims.Add(new Sphere(new Point3(4, 1, 0), 1, new Metal(0.7, 0.6, 0.5)));

            // just for the sake of writing some slightly more modern c#

            IMaterial RandomMaterial()
            {
                var r = Sampler.Instance.Random01();
                return r switch
                {
                    < 0.8 => new Lambertian(Sampler.Instance.RandomColor() * Sampler.Instance.RandomColor()),
                    < 0.95 => new Metal(Sampler.Instance.RandomColor(0.5f, 1.0f), Sampler.Instance.Random(0.0, 0.05)),
                    _ => new Dielectric(1.5)
                };
            }

            var offLimitsZone = new Point3(4, 0.2, 0);

            prims.AddRange(Enumerable
                .Range(-11, 22)
                .SelectMany(a => Enumerable.Range(-11, 22).Select(b => (a, b)))
                .Select(tpl =>
                    new Point3(
                        tpl.a + 0.9 * Sampler.Instance.Random01(),
                        0.2,
                        tpl.b + 0.9 * Sampler.Instance.Random01()))
                .Where(center => (center - offLimitsZone).Length() > 0.9)
                .Select(center => new Sphere(center, 0.2, RandomMaterial())));

            var orientation = new Orientation
            {
                LookFrom = new Point3(13, 2, 3),
                LookAt = new Point3(0, 0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 20.0, .1, 10.0);

            return new Scene
            {
                World = new BVHTree(prims),
                Camera = cam
            };
        }
    }
}