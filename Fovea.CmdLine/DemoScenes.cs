using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Image;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Primitives;
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
        public const float AspectRatio = 16.0f / 9.0f;
        
        public static Scene MakeScene(DemoScenes sceneId, int imageWidth)
        {
            var scene = sceneId switch
            {
                DemoScenes.FinalSceneBookOne => GetFinalSceneBookOne(),
                _ => GetHollowGlassScene()
            };

            scene.OutputSize = new ImageSize(imageWidth, (int)(imageWidth / AspectRatio), AspectRatio);
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
            var cam = new PerspectiveCamera(orientation, AspectRatio, 90.0f, .1f, focusDist);

            return new Scene
            {
                World = new PrimitiveList(prims),
                Camera = cam
            };
        }

        private static Scene GetFinalSceneBookOne()
        {
            throw new System.NotImplementedException();
        }
    }
}