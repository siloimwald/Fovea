using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Fovea.Renderer.Image;
using static System.MathF;

namespace Fovea.Renderer.Sampling
{
    /// <summary>
    ///     random sampling utility class. replaced all of the rejection sampling methods with direct approaches from
    ///     various sources
    /// </summary>
    public class Sampler
    {
        public static readonly Sampler Instance = new();

        // TODO: fixed seed while testing, remove this
        private readonly ThreadLocal<Random> _random = new(() => new Random());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Random01()
        {
            return _random.Value.NextSingle();
        }

        public float Random(float min, float max)
        {
            return min + (max - min) * Random01();
        }

        public int RandomInt(int min, int max)
        {
            return _random.Value.Next(min, max);
        }

        public (float px, float py) RandomOnUnitDisk()
        {
            // found all over the internets, not perfectly uniformly distributed, but good enough
            var r = Sqrt(Random01());
            var theta = 2.0f * PI * Random01();
            return (r * Cos(theta), r * Sin(theta));
        }

        /// <summary>generate a random point on the surface of a unit sphere from the GI Compendium</summary>
        /// <returns></returns>
        public Vector3 RandomOnUnitSphere()
        {
            var r1 = Random01();
            var r2 = Random01();
            var r = 2.0f * Sqrt(r2 * (1.0f - r2));
            var phi = 2.0f * PI * r1;
            return new Vector3(r * Cos(phi), r * Sin(phi), 1.0f - 2.0f * r2);
        }

        /// <summary>Sampling the hemisphere (isn't that the same as on unit sphere?) p(direction) = cos(phi)/pi</summary>
        /// <returns></returns>
        public Vector3 RandomCosineDirection()
        {
            var r1 = Random01();
            var r2 = Random01();
            var z = Sqrt(1 - r2);
            var phi = 2 * PI * r1;
            var r2Sqrt = Sqrt(r2);
            return new Vector3(Cos(phi) * r2Sqrt, Sin(phi) * r2Sqrt, z);
        }

        public RGBColor RandomColor(float min = 0.0f, float max = 1.0f)
        {
            return new(Random(min, max), Random(min, max), Random(min, max));
        }

    }
}