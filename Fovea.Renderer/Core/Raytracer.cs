using System;
using System.Diagnostics;
using Fovea.Renderer.Image;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using Fovea.Renderer.Viewing;

namespace Fovea.Renderer.Core
{
    public class Raytracer
    {
        private PrimitiveList _scene;

        public int MaxDepth { get; } = 40;
        public int NumSamples { get; } = 500;
        
        public Raytracer()
        {
            _scene = new PrimitiveList();
            var centerMaterial = new Lambertian(0.1f, 0.2f, 0.5f);
            var groundMaterial = new Lambertian(0.8f, 0.8f, 0.0f);
            var materialLeft = new Dielectric(1.5f);
            var materialRight = new Metal(0.8f, 0.6f, 0.2f, 0.1f);
            _scene.Add(new Sphere(new Point3( 0, 0, -1), 0.5f, centerMaterial));
            _scene.Add(new Sphere(new Point3( 0, -100.5f, -1), 100, groundMaterial));
            _scene.Add(new Sphere(new Point3(-1, 0, -1), 0.5f, materialLeft));
            _scene.Add(new Sphere(new Point3(-1, 0, -1), -0.45f, materialLeft));
            _scene.Add(new Sphere(new Point3( 1, 0, -1), 0.5f, materialRight));
        }
        
        private RGBColor ColorRay(Ray ray, int depth)
        {
            if (depth <= 0)
                return new RGBColor(0.0f);
            
            var hitRecord = new HitRecord();
            if (_scene.Hit(ray, 1e-4f, float.PositiveInfinity, hitRecord))
            {
                var scatterResult = new ScatterResult();
                if (hitRecord.Material.Scatter(ray, hitRecord, scatterResult))
                {
                    return scatterResult.Attenuation * ColorRay(scatterResult.OutgoingRay, depth - 1);    
                }
                return new RGBColor();
            }

            var normalizedDir = Vec3.Normalize(ray.Direction);
            var t = 0.5f * (normalizedDir.Y + 1);
            return new RGBColor(1.0f) * (1.0f - t) + new RGBColor(0.5f, 0.7f, 1.0f) * t;
        }

        public void Render()
        {
            // Image
            var aspectRatio = 16.0f / 9.0f;
            var imageWidth = 400;
            var imageHeight = (int) (imageWidth / aspectRatio);
            var image = new ImageFilm(imageWidth, imageHeight);

            // Camera
            var orientation = new Orientation
            {
                LookAt = new Point3(0,0, -1),
                LookFrom = new Point3(-2, 2, 1),
                UpDirection = new Vec3(0, 1, 0)
            };

            // print what we're doing
            Console.WriteLine($"Image size {imageWidth}x{imageHeight}, samples = {NumSamples}");
            
            var cam = new PerspectiveCamera(orientation, aspectRatio, 20.0f);
            var sw = Stopwatch.StartNew(); 
            for (var px = 0; px < imageWidth; ++px)
            {
                for (var py = 0; py < imageHeight; ++py)
                {
                    var color = new RGBColor();
                    for (var s = 0; s < NumSamples; ++s)
                    {
                        var u = (px + Sampler.Instance.Random01()) / (imageWidth - 1);
                        var v = (py + Sampler.Instance.Random01()) / (imageHeight - 1);
                        var ray = cam.ShootRay(u, v);
                        color += ColorRay(ray, MaxDepth);
                            
                    }
                    image[(px, imageHeight - py - 1)] = color;
                }
            }
            // average and gamma correct whole image in one go
            image.Average(NumSamples);
            Console.WriteLine($"Finished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
            image.SaveAs("output.ppm");
        }
    }
}