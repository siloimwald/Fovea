﻿using System;
using Fovea.Renderer.Image;
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
        public int NumSamples { get; } = 100;
        
        public Raytracer()
        {
            _scene = new PrimitiveList();
            _scene.Add(new Sphere(new Point3(0, 0, -1), 0.5f));
            _scene.Add(new Sphere(new Point3(0, -100.5f, -1), 100));
        }
        
        RGBColor ColorRay(Ray ray, int depth)
        {
            if (depth <= 0)
                return new RGBColor(0.0f);
            
            var hitRecord = new HitRecord();
            if (_scene.Hit(ray, 1e-4f, float.PositiveInfinity, hitRecord))
            {
                var target = hitRecord.HitPoint + hitRecord.Normal + Sampler.Instance.RandomOnUnitSphere();
                var nextRay = new Ray(hitRecord.HitPoint, target - hitRecord.HitPoint);
                return ColorRay(nextRay, depth - 1) * 0.5f;
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
                LookAt = new Point3(0, 0, -1),
                LookFrom = new Point3(),
                UpDirection = new Vec3(0, 1, 0)
            };

            var cam = new PerspectiveCamera(orientation, aspectRatio, 90.0f);
            
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
            image.SaveAs("output.ppm");
        }
    }
}