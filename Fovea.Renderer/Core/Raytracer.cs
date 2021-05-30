using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fovea.Renderer.Image;
using Fovea.Renderer.Sampling;

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

            // a simple image partitioning scheme, each thread renders chunks of 'pixelPerThread'
            // many pixels. the next chunk is found by simply skipping the chunks of all other threads

            const int pixelPerThread = 10;
            var threadCount = Environment.ProcessorCount;
            var totalPixels = imageHeight * imageWidth;
            var pixelDone = 0;

            void RenderInterleaved(int taskNum)
            {
                var pixelBufferStart = taskNum * pixelPerThread;
                var increment = pixelPerThread * threadCount;
                for (var offset = pixelBufferStart; offset < totalPixels; offset += increment)
                {
                    var max = Math.Min(offset + pixelPerThread, totalPixels);
                    for (var p = offset; p < max; p++)
                    {
                        var py = Math.DivRem(p, imageWidth, out var px);
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

                    Interlocked.Add(ref pixelDone, max - offset);

                    if (taskNum != 0) continue;

                    var percent = pixelDone / (double) totalPixels * 100.0;
                    Console.Write($"\r{percent:#.00}% done  ");
                }
            }

            Parallel.For(0, threadCount,
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, RenderInterleaved);

            // average and gamma correct whole image in one go
            image.Average(NumSamples);
            Console.WriteLine($"\nFinished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
            image.SaveAs("output.ppm");
        }
    }
}