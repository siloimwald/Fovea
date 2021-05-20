using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public int MaxDepth { get; set; } = 50;
        public int NumSamples { get; set; } = 100;

        private RGBColor ColorRay(Ray ray, IPrimitive world, int depth)
        {
            if (depth <= 0)
                return new RGBColor(0.0);
            
            var hitRecord = new HitRecord();
            if (world.Hit(ray, 1e-4, double.PositiveInfinity, ref hitRecord))
            {
                var scatterResult = new ScatterResult();
                if (hitRecord.Material.Scatter(ray, hitRecord, ref scatterResult))
                {
                    return scatterResult.Attenuation * ColorRay(scatterResult.OutgoingRay, world, depth - 1);    
                }
                return new RGBColor();
            }

            var normalizedDir = Vec3.Normalize(ray.Direction);
            var t = 0.5 * (normalizedDir.Y + 1);
            return new RGBColor(1.0) * (1.0 - t) + new RGBColor(0.5, 0.7, 1.0) * t;
        }

        public void Render(Scene scene)
        {
            var (imageWidth, imageHeight) = scene.OutputSize;
            var image = new ImageFilm(imageWidth, imageHeight);
            
            // print what we're doing
            Console.WriteLine($"Image size {imageWidth}x{imageHeight}, samples = {NumSamples}");
            
            var sw = Stopwatch.StartNew();

            void RenderScanLine(int py)
            {
                for (var px = 0; px < imageWidth; ++px)
                {
                    var color = new RGBColor();
                    for (var s = 0; s < NumSamples; ++s)
                    {
                        var u = (px + Sampler.Instance.Random01()) / (imageWidth - 1);
                        var v = (py + Sampler.Instance.Random01()) / (imageHeight - 1);
                        var ray = scene.Camera.ShootRay(u, v);
                        color += ColorRay(ray, scene.World, MaxDepth);
                            
                    }
                    image[(px, imageHeight - py - 1)] = color;
                }
            }

            Parallel.For(0, imageHeight, RenderScanLine);
            
            // average and gamma correct whole image in one go
            image.Average(NumSamples);
            Console.WriteLine($"Finished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
            image.SaveAs("output.ppm");
        }
    }
}