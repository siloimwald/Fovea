using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Fovea.Renderer.Image;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Sampling
{
    /// <summary>
    /// random sampling utility class. replaced all of the rejection sampling methods
    /// with direct approaches from various sources
    /// </summary>
    public class Sampler
    {
        public static readonly Sampler Instance = new();
        private readonly ThreadLocal<Random> _random = new(() => new Random(0xAAFFEE));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Random01() => (float) _random.Value.NextDouble();

        public float Random(float min, float max) => min + (max - min) * Random01();

        public (float px, float py) RandomOnUnitDisk()
        {
            // found all over the internets, but perfectly uniformly distributed, but good enough
            var r = Sqrt(Random01());
            var theta = 2.0f * PI * Random01();
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
            var r = 2.0f * Sqrt(r2 * (1.0f - r2));
            var phi = 2.0f * PI * r1;
            return new Vec3(r * Cos(phi), r * Sin(phi), 1.0f - 2.0f * r2);
        }

        public RGBColor RandomColor(float min = 0.0f, float max = 1.0f) 
            => new(Random(min, max), Random(min, max), Random(min, max));
    }
}