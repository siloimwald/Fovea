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

    private RGBColor ColorRay(in Ray ray, Scene scene, int depth)
    {
        if (depth <= 0)
            return RGBColor.Black;
        
        var hitRecord = new HitRecord();

        // no hit
        if (!scene.World.Hit(ray, new Interval(1e-3f, float.PositiveInfinity), ref hitRecord))
        {
            return scene.Background;
        }
        
        var colorFromEmission = hitRecord.Material.Emitted(ray, hitRecord);
            
        var scatterResult = new ScatterResult();

        // no scattering
        if (!hitRecord.Material.Scatter(ray, hitRecord, ref scatterResult))
        {
            return colorFromEmission;
        }

        if (scatterResult.IsSpecular)
        {
            return scatterResult.Attenuation 
                   * ColorRay(scatterResult.SpecularRay, scene, depth - 1);
        }
        
        var lightPdf = new PrimitivePDF(scene.ImportanceSamplingList, hitRecord.HitPoint);
        var mixturePdf = new MixturePDF(lightPdf, scatterResult.Pdf);
        
        var scatteredDirection = new Ray(hitRecord.HitPoint, mixturePdf.Generate(), ray.Time);
        var pdfValue = mixturePdf.Evaluate(scatteredDirection.Direction);
        
        // from actual primitive
        var scatteringPdf = hitRecord.Material.ScatteringPDF(ray, hitRecord, scatteredDirection);

        var colorFromScatter
            = (scatterResult.Attenuation * scatteringPdf *
               ColorRay(scatteredDirection, scene, depth - 1)) *  (1.0f /pdfValue);

        return colorFromEmission + colorFromScatter;
    }

    public void Render(Scene scene)
    {
        var imageWidth = scene.Options.ImageWidth;
        var imageHeight = scene.Options.ImageHeight;
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
            var sqrtSpp = (int)MathF.Sqrt(scene.Options.NumSamples);
            for (var offset = pixelBufferStart; offset < totalPixels; offset += increment)
            {
                var max = Math.Min(offset + pixelPerThread, totalPixels);
                for (var p = offset; p < max; p++)
                {
                    var py = Math.DivRem(p, imageWidth, out var px);
                    var color = new RGBColor();
                    
                    for (var sj = 0; sj < sqrtSpp ; sj++) {
                        for (var si = 0; si < sqrtSpp; si++) {
                            var ray = scene.Camera.ShootRay(px, py, si, sj);
                            color += ColorRay(ray, scene, scene.Options.MaxDepth);
                        }
                    }
                    
                    // ReSharper disable once AccessToDisposedClosure
                    image[px, py] = new RgbaVector(color.R, color.G, color.B);                    
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