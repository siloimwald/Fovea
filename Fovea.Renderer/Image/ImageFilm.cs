using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Fovea.Renderer.Image
{
    /// <summary>abstraction for a image buffer/image file. support for saving to ppm format</summary>
    public class ImageFilm
    {
        private readonly RGBColor[] _imageBuffer;

        /// <summary>create a new image film with a given dimension</summary>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <exception cref="ArgumentException">invalid size values provided</exception>
        public ImageFilm(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException($"invalid image dimensions {width}x{height}");
            Width = width;
            Height = height;

            _imageBuffer = new RGBColor[width * height];
        }

        public int Width { get; }
        public int Height { get; }

        public RGBColor this[(int px, int py) pixelPos]
        {
            get => _imageBuffer[Width * pixelPos.py + pixelPos.px];
            set => _imageBuffer[Width * pixelPos.py + pixelPos.px] = value;
        }

        /// <summary>saves the image buffer to the given file name in ppm format</summary>
        /// <param name="fileName">name of output file</param>
        public void SaveAs(string fileName)
        {
            var header = $"P6 {Width} {Height} 255\n";
            var headerAsByteArray = Encoding.ASCII.GetBytes(header);
            // this relies on the iteration order here somehow...
            var imageAsByteArray = _imageBuffer.SelectMany(pixelColor => pixelColor.ToByteArray());
            File.WriteAllBytes(fileName, headerAsByteArray.Concat(imageAsByteArray).ToArray());
        }

        public void Average(int numSamples)
        {
            var scale = 1.0f / numSamples; // average samples
            // square root for a rough gamma correction approximation (~ Gamma = 2)
            for (var cIdx = 0; cIdx < _imageBuffer.Length; ++cIdx)
            {
                var colorAtIndex = _imageBuffer[cIdx];
                _imageBuffer[cIdx] = new RGBColor(
                    MathF.Sqrt(colorAtIndex.R * scale),
                    MathF.Sqrt(colorAtIndex.G * scale),
                    MathF.Sqrt(colorAtIndex.B * scale));
            }
        }
    }
}