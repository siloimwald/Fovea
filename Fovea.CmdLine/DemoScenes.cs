using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Core.BVH;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.Primitives.CSG;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.Viewing;

namespace Fovea.CmdLine
{
    public enum DemoScenes
    {
        FinalSceneBookOne,
        CSGTest,
        HollowGlass
    }
    
    public static class DemoSceneCreator
    {
        private const double DefaultAspectRatio = 16.0 / 9.0;
        
        public static Scene MakeScene(DemoScenes sceneId, int imageWidth)
        {
            var scene = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => GetFinalSceneBookOne(),
                DemoScenes.CSGTest => GetCSGTestScene(),
                _ => GetHollowGlassScene()
            };
            
            scene.OutputSize = (imageWidth, (int)(imageWidth / DefaultAspectRatio));
            return scene;
        }

        private static Scene GetCSGTestScene()
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
                LookAt = new Point3(0,0, -1),
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
                LookFrom = new Point3(0,0, 0),
                LookAt = new Point3(0,0, -1),
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
            prims.Add(new Sphere(new Point3(0, 1,0), 1, new Dielectric(1.5)));
            prims.Add(new Sphere(new Point3(-4, 1, 0), 1, new Lambertian(0.4, 0.2, 0.1)));
            prims.Add(new Sphere(new Point3( 4, 1, 0), 1, new Metal(0.7, 0.6, 0.5)));
            
            // just for the sake of writing some slightly more modern c#
            
            IMaterial RandomMaterial()
            {
                var r = Sampler.Instance.Random01();
                return r switch
                {
                    < 0.8 => new Lambertian(Sampler.Instance.RandomColor()*Sampler.Instance.RandomColor()),
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
                LookAt = new Point3(0,0, 0),
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