using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;
using static System.Math;

namespace Fovea.Renderer.Sampling
{
    /// <summary>
    /// random sampling utility class. replaced all of the rejection sampling methods
    /// with direct approaches from various sources
    /// </summary>
    public class Sampler
    {
        public static readonly Sampler Instance = new();
        private readonly ThreadLocal<Random> _random = new(() => new Random());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Random01() => _random.Value.NextDouble();

        public double Random(double min, double max) => min + (max - min) * Random01();

        public int RandomInt(int min, int max) => _random.Value.Next(min, max);

        public (double px, double py) RandomOnUnitDisk()
        {
            // found all over the internets, not perfectly uniformly distributed, but good enough
            var r = Sqrt(Random01());
            var theta = 2.0 * PI * Random01();
            return (r * Cos(theta), r * Sin(theta));
        }

        /// <summary>
        /// generate a random point on the surface of a unit sphere
        /// from the GI Compendium
        /// </summary>
        /// <returns></returns>
        public Vec3 RandomOnUnitSphere()
        {
            var r1 = Random01();
            var r2 = Random01();
            var r = 2.0 * Sqrt(r2 * (1.0 - r2));
            var phi = 2.0 * PI * r1;
            return new Vec3(r * Cos(phi), r * Sin(phi), 1.0 - 2.0 * r2);
        }

        public RGBColor RandomColor(double min = 0.0, double max = 1.0)
            => new(Random(min, max), Random(min, max), Random(min, max));
    }
}