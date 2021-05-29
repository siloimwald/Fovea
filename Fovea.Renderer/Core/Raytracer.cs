using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public class Raytracer
    {
        public int MaxDepth { get; set; } = 50;
        public int NumSamples { get; set; } = 100;
        
        private RGBColor ColorRay(Ray ray, Scene scene, int depth)
        {
            if (depth <= 0)
                return new RGBColor(0.0);
            
            var hitRecord = new HitRecord();

            if (!scene.World.Hit(ray, 1e-4, double.PositiveInfinity, ref hitRecord))
                return scene.Background;
            
            var scatterResult = new ScatterResult();
            var emitted = hitRecord.Material.Emitted(hitRecord.TextureU, hitRecord.TextureV, hitRecord.HitPoint);
            if (!hitRecord.Material.Scatter(ray, hitRecord, ref scatterResult))
                return emitted;

            return emitted 
                   + scatterResult.Attenuation
                   * ColorRay(scatterResult.OutgoingRay, scene, depth - 1);
        }

        public void Render(Scene scene)
        {
            var (imageWidth, imageHeight) = scene.OutputSize;
            var image = new ImageFilm(imageWidth, imageHeight);
            
            // print what we're doing
            Console.WriteLine($"Image size {imageWidth}x{imageHeight}, samples = {NumSamples}");
            
            var sw = Stopwatch.StartNew();

            var linesDone = 0;
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
                        color += ColorRay(ray, scene, MaxDepth);
                            
                    }
                    image[(px, imageHeight - py - 1)] = color;
                }

                lock (this)
                {
                    linesDone++;
                    var p = linesDone / (double)imageHeight * 100.0;
                    Console.Write($"\rDone {p:#.##}  ");
                }
            }

            Parallel.For(0, imageHeight, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, RenderScanLine);
            
            // average and gamma correct whole image in one go
            image.Average(NumSamples);
            Console.WriteLine($"\nFinished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
            image.SaveAs("output.ppm");
        }
    }
}