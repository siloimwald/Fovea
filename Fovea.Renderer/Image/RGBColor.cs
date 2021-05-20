using System;


namespace Fovea.Renderer.Image
{
    public struct RGBColor
    {
        public double R;
        public double G;
        public double B;

        public RGBColor(double s = 0.0) : this(s, s, s)
        {
        }

        public RGBColor(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        // addition
        public static RGBColor operator +(RGBColor left, RGBColor right)
        {
            return new(left.R + right.R, left.G + right.G, left.B + right.B);
        }

        // scalar multiplication
        public static RGBColor operator *(RGBColor color, double s)
        {
            return new(color.R * s, color.G * s, color.B * s);
        }

        /// <summary>
        /// mix color left and right by component-wise multiplication
        /// </summary>
        /// <returns></returns>
        public static RGBColor operator *(RGBColor left, RGBColor right)
        {
            return new(left.R * right.R, left.G * right.G, left.B * right.B);
        }

        /// <summary>
        /// clamps color components to [0..1], scales by 255 and packs
        /// components as an byte array with form [r,g,b]
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return new[]
            {
                (byte) (Math.Clamp(R, 0.0, 1.0) * 255.999),
                (byte) (Math.Clamp(G, 0.0, 1.0) * 255.999),
                (byte) (Math.Clamp(B, 0.0, 1.0) * 255.999)
            };
        }
        
    }
}