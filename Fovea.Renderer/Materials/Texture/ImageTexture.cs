using System;
using System.Drawing;
using System.IO;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials.Texture
{
    public class ImageTexture : ITexture
    {
        private readonly ImageFilm _imageBuffer;

        public ImageTexture(string fileName)
        {
            try
            {
                using var stream = new FileStream(fileName, FileMode.Open);
                using var image = new Bitmap(stream);
                _imageBuffer = new ImageFilm(image.Width, image.Height);
                for (var px = 0; px < image.Width; ++px)
                for (var py = 0; py < image.Height; ++py)
                {
                    var c = image.GetPixel(px, py);
                    var rgb = new RGBColor(c.R / 255.0, c.G / 255.0, c.B / 255.0);
                    // flip y
                    _imageBuffer[(px, image.Height - py - 1)] = rgb;
                }
            }
            catch
            {
                Console.WriteLine($"failed to read {fileName}");
            }
        }

        public RGBColor Value(double u, double v, Point3 p)
        {
            if (_imageBuffer == null)
                return new RGBColor(0, 1, 1);

            var texU = Math.Clamp(u, 0.0, 1.0);
            var texV = Math.Clamp(v, 0.0, 1.0);
            var px = (int) (texU * (_imageBuffer.Width - 1));
            var py = (int) (texV * (_imageBuffer.Height - 1));
            return _imageBuffer[(px, py)];
        }
    }
}