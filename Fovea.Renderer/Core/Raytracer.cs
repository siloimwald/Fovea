using System;
using System.Diagnostics;
using System.Threading;
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
            // nothing hit, yield background
            if (!scene.World.Hit(ray, Interval.HalfOpenWithOffset(), ref hitRecord))
            {
                if (scene.Environment != null &&
                    scene.Environment.Hit(ray, Interval.HalfOpenWithOffset(), ref hitRecord))
                {
                    var envScatter = new ScatterResult();
                    if (hitRecord.Material.Scatter(ray, hitRecord, ref envScatter))
                        return envScatter.Attenuation;
                }

                return scene.Background;
            }


            var scatterResult = new ScatterResult();
            var emitted = hitRecord.Material.Emitted(ray, hitRecord);
            // no scattering takes place or hit emitter, return emitted
            if (!hitRecord.Material.Scatter(ray, hitRecord, ref scatterResult))
                return emitted;

            if (scatterResult.IsSpecular)
                return scatterResult.Attenuation * ColorRay(scatterResult.SpecularRay, scene, depth - 1);

            // attempt at being compatible with the previous book scenes
            if (scene.Lights == null)
            {
                var outRay = new Ray(hitRecord.HitPoint, scatterResult.Pdf.Generate());
                return emitted
                       + scatterResult.Attenuation
                       * hitRecord.Material.ScatteringPDF(ray, hitRecord, outRay)
                       * ColorRay(outRay, scene, depth - 1) * (1.0 / scatterResult.Pdf.Evaluate(outRay.Direction.AsVector3()));
            }
            else
            {
                var lightPdf = new PrimitivePDF(scene.Lights, hitRecord.HitPoint);
                var mixPdf = new MixturePDF(scatterResult.Pdf, lightPdf);
                var outRay = new Ray(hitRecord.HitPoint, mixPdf.Generate(), ray.Time);
                var pdfCorrection = 1.0 / mixPdf.Evaluate(outRay.Direction.AsVector3());

                return emitted
                       + scatterResult.Attenuation
                       * hitRecord.Material.ScatteringPDF(ray, hitRecord, outRay)
                       * ColorRay(outRay, scene, depth - 1) * pdfCorrection;
            }
        }

        public void Render(Scene scene, string fileName = "output.ppm")
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
                            var r1 = Sampler.Instance.Random01();
                            var r2 = Sampler.Instance.Random01();
                            var u = r1 + px / ((double)imageWidth - 1);
                            var v = r2 + py / ((double)imageHeight - 1);
                            u = -1.0 + 2 * (u - r1);
                            v = -1.0 + 2 * (v - r2);
                            var ray = scene.Camera.ShootRay((float)u, (float)v);
                            color += ColorRay(ray, scene, MaxDepth);
                        }

                        image[(px, imageHeight - py - 1)] = color;
                    }

                    Interlocked.Add(ref pixelDone, max - offset);

                    if (taskNum != 0) continue;

                    var percent = pixelDone / (double) totalPixels * 100.0;
                    Console.Write($"\r{percent:000.0}% done  ");
                }
            }

            Parallel.For(0, threadCount,
                new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, RenderInterleaved);

            // average and gamma correct whole image in one go
            image.Average(NumSamples);
            Console.WriteLine($"\nFinished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
            image.SaveAs(fileName);
        }
    }
}