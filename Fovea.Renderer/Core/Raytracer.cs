using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Fovea.Renderer.Sampling;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Fovea.Renderer.Core;

public class Raytracer
{
    
    private static readonly ILogger<Raytracer> Log = Logging.GetLogger<Raytracer>();
    
    private RGBColor ColorRay(Ray ray, Scene scene, int depth)
    {
        if (depth <= 0)
            return new RGBColor(0.0f);

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
        // if (scene.Lights == null)
        // {
        //     var outRay = new Ray(hitRecord.HitPoint, scatterResult.Pdf.Generate());
        //     return emitted
        //            + scatterResult.Attenuation
        //            * hitRecord.Material.ScatteringPDF(ray, hitRecord, outRay)
        //            * ColorRay(outRay, scene, depth - 1) * (1.0f / scatterResult.Pdf.Evaluate(outRay.Direction));
        // }
        // else
        {
            var lightPdf = new PrimitivePDF(scene.Lights, hitRecord.HitPoint);
            var mixPdf = new MixturePDF(scatterResult.Pdf, lightPdf);
            var outRay = new Ray(hitRecord.HitPoint, mixPdf.Generate(), ray.Time);
            var pdfCorrection = 1.0f / mixPdf.Evaluate(outRay.Direction);

            return emitted
                   + scatterResult.Attenuation
                   * hitRecord.Material.ScatteringPDF(ray, hitRecord, outRay)
                   * ColorRay(outRay, scene, depth - 1) * pdfCorrection;
        }
    }

    public void Render(Scene scene)
    {
        var imageWidth = scene.Options.ImageWidth;
        var imageHeight = scene.Options.ImageHeight;
        // var image = new ImageFilm(imageWidth, imageHeight);
        var image = new Image<RgbaVector>(imageWidth, imageHeight);

        // print what we're doing
        Log.LogInformation("Image size {imageWidth}x{imageHeight}, samples = {numSamples}",
            imageWidth, imageHeight, scene.Options.NumSamples);
        
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
                    for (var s = 0; s < scene.Options.NumSamples; ++s)
                    {
                        var r1 = Sampler.Instance.Random01();
                        var r2 = Sampler.Instance.Random01();
                        var u = r1 + px / ((float)imageWidth - 1);
                        var v = r2 + py / ((float)imageHeight - 1);
                        u = -1.0f + 2 * (u - r1);
                        v = -1.0f + 2 * (v - r2);
                        var ray = scene.Camera.ShootRay(u, v);
                        color += ColorRay(ray, scene, scene.Options.MaxDepth);
                    }
                    
                    // ReSharper disable once AccessToDisposedClosure
                    image[px, imageHeight - py - 1] = new RgbaVector(color.R, color.G, color.B);                    
                }

                Interlocked.Add(ref pixelDone, max - offset);

                if (taskNum != 0) continue; // make only one task print progress

                var percent = pixelDone / (float) totalPixels * 100.0;
                Console.Write($"\r{percent:000.0}% done  ");
            }
        }

        Parallel.For(0, threadCount,
            new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, RenderInterleaved);
        
        // average and gamma correct whole image in one go
        image.Mutate(c => c.ProcessPixelRowsAsVector4(row =>
        {
            for (var x = 0; x < row.Length; x++)
            {
                row[x] = Vector4.Divide(row[x], scene.Options.NumSamples);
                row[x] = Vector4.SquareRoot(row[x]);
                row[x].W = 1.0f; // fix the alpha channel
            }
        }));
        
        Console.WriteLine($"\nFinished rendering in {sw.Elapsed.TotalSeconds:0.##} secs.");
        image.SaveAsPng(scene.Options.OutputFile);
        image.Dispose();
    }
}