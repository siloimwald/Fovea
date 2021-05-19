using System;
using System.Collections.Generic;
using System.Linq;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.Viewing;

namespace Fovea.CmdLine
{
    public enum DemoScenes
    {
        FinalSceneBookOne,
        HollowGlass
    }
    
    public static class DemoSceneCreator
    {
        private const float DefaultAspectRatio = 16.0f / 9.0f;
        
        public static Scene MakeScene(DemoScenes sceneId, int imageWidth)
        {
            var scene = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => GetFinalSceneBookOne(),
                _ => GetHollowGlassScene()
            };

            var ar = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => 3.0 / 2.0,
                _ => DefaultAspectRatio
            };
            
            scene.OutputSize = (imageWidth, (int)(imageWidth / ar));
            return scene;
        }

        private static Scene GetHollowGlassScene()
        {
            var centerMaterial = new Lambertian(0.1f, 0.2f, 0.5f);
            var groundMaterial = new Lambertian(0.8f, 0.8f, 0.0f);
            var materialLeft = new Dielectric(1.5f);
            var materialRight = new Metal(0.8f, 0.6f, 0.2f, 0.1f);
            var prims = new List<IPrimitive>
            {
                new Sphere(new Point3(0, 0, -1), 0.5f, centerMaterial),
                new Sphere(new Point3(0, -100.5f, -1), 100, groundMaterial),
                new Sphere(new Point3(-1, 0, -1), 0.5f, materialLeft),
                new Sphere(new Point3(-1, 0, -1), -0.45f, materialLeft),
                new Sphere(new Point3(1, 0, -1), 0.5f, materialRight)
            };
            
            // Camera
            var orientation = new Orientation
            {
                LookFrom = new Point3(0,0, 0),
                LookAt = new Point3(0,0, -1),
                UpDirection = new Vec3(0, 1, 0)
            };
            
            var focusDist = (orientation.LookFrom - orientation.LookAt).Length();
            var cam = new PerspectiveCamera(orientation, DefaultAspectRatio, 90.0f, .1f, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetFinalSceneBookOne()
        {
            var prims = new List<IPrimitive>();
            var groundMat = new Lambertian(0.5f, 0.5f, 0.5f);
            prims.Add(new Sphere(new Point3(0, -1000, 0), 1000, groundMat));
            prims.Add(new Sphere(new Point3(0, 1,0), 1, new Dielectric(1.5f)));
            prims.Add(new Sphere(new Point3(-4, 1, 0), 1, new Lambertian(0.4f, 0.2f, 0.1f)));
            prims.Add(new Sphere(new Point3( 4, 1, 0), 1, new Metal(0.7f, 0.6f, 0.5f)));
            
            // just for the sake of writing some slightly more modern c#
            
            IMaterial RandomMaterial()
            {
                var r = Sampler.Instance.Random01();
                return r switch
                {
                    < 0.8f => new Lambertian(Sampler.Instance.RandomColor()*Sampler.Instance.RandomColor()),
                    < 0.95f => new Metal(Sampler.Instance.RandomColor(0.5f, 1.0f), Sampler.Instance.Random(0.0f, 0.05f)),
                    _ => new Dielectric(1.5f)
                };
            }
            
            var offLimitsZone = new Point3(4, 0.2f, 0);
            
            prims.AddRange(Enumerable
                .Range(-11, 22)
                .SelectMany(a => Enumerable.Range(-11, 22).Select(b => (a, b)))
                .Select(tpl =>
                    new Point3(
                        tpl.a + 0.9f * Sampler.Instance.Random01(),
                        0.2f,
                        tpl.b + 0.9f * Sampler.Instance.Random01()))
                .Where(center => (center - offLimitsZone).Length() > 0.9f)
                .Select(center => new Sphere(center, 0.2f, RandomMaterial())));

            var orientation = new Orientation
            {
                LookFrom = new Point3(13, 2, 3),
                LookAt = new Point3(0,0, 0),
                UpDirection = new Vec3(0, 1, 0)
            };

            const float ar = 3.0f / 2.0f;
            var cam = new PerspectiveCamera(orientation, ar, 20.0f, .1f, 10.0f);
            
            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }
    }
}