using System;
using Fovea.Renderer.Image;
using Fovea.Renderer.Primitives;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public class Raytracer
    {
        private PrimitiveList _scene;

        public Raytracer()
        {
            _scene = new PrimitiveList();
            _scene.Add(new Sphere(new Point3(0, 0, -1), 0.5f));
            _scene.Add(new Sphere(new Point3(0, -100.5f, -1), 100));
        }
        
        RGBColor ColorRay(Ray ray)
        {
            var hitRecord = new HitRecord();
            if (_scene.Hit(ray, 1e-4f, float.PositiveInfinity, hitRecord))
            {
                return new RGBColor(hitRecord.Normal.X + 1, hitRecord.Normal.Y + 1, hitRecord.Normal.Z + 1) * 0.5f;
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
            var viewportHeight = 2.0f;
            var viewportWidth = aspectRatio * viewportHeight;
            var focalLength = 1.0f;

            var origin = new Point3(0.0f);
            var horizontal = new Vec3(viewportWidth, 0, 0);
            var vertical = new Vec3(0, viewportHeight, 0);
            var lowerLeftCorner = origin - horizontal * 0.5f - vertical * 0.5f - new Vec3(0, 0, focalLength);

            for (var px = 0; px < imageWidth; ++px)
            {
                for (var py = 0; py < imageHeight; ++py)
                {
                    var u = (float)px / (imageWidth - 1);
                    var v = (float) py / (imageHeight - 1);
                    var ray = new Ray(origin, lowerLeftCorner + horizontal * u + vertical * v - origin);
                    var c = ColorRay(ray);
                    image[(px, imageHeight - py - 1)] = c;
                }
            }
            
            image.SaveAs("output.ppm");
        }
    }
}