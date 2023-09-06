﻿using System;
using System.Linq;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Materials.Texture
{
    /// <summary>
    ///     perlin noise from 'ray tracing the next week'. No idea how this all works in detail, but it does produce nice
    ///     looking stuff ;)
    /// </summary>
    public class Perlin
    {
        private const int PointCount = 256;
        private readonly int[] _permutationX;
        private readonly int[] _permutationY;
        private readonly int[] _permutationZ;

        private readonly Vec3[] _randomVectors;

        public Perlin()
        {
            _randomVectors = new Vec3[PointCount];
            // grab us some random doubles
            for (var i = 0; i < _randomVectors.Length; ++i)
                _randomVectors[i] = Sampler.Instance.RandomOnUnitSphere();

            // create three different permutations of indices
            _permutationX = Enumerable.Range(0, PointCount).ToArray();
            _permutationY = Enumerable.Range(0, PointCount).ToArray();
            _permutationZ = Enumerable.Range(0, PointCount).ToArray();
            Permute(_permutationX);
            Permute(_permutationY);
            Permute(_permutationZ);
        }

        private double Noise(Vector3 p)
        {
            var fpx = Math.Floor(p.X);
            var fpy = Math.Floor(p.Y);
            var fpz = Math.Floor(p.Z);

            var u = p.X - fpx;
            var v = p.Y - fpy;
            var w = p.Z - fpz;

            var i = (int) fpx;
            var j = (int) fpy;
            var k = (int) fpz;

            var c = new Vec3[2, 2, 2];

            for (var di = 0; di < 2; ++di)
            for (var dj = 0; dj < 2; dj++)
            for (var dk = 0; dk < 2; dk++)
                c[di, dj, dk] = _randomVectors[
                    _permutationX[(i + di) & 255] ^
                    _permutationY[(j + dj) & 255] ^
                    _permutationZ[(k + dk) & 255]
                ];

            return TriLinearInterpolate(c, u, v, w);
        }

        public double Turbulence(Vector3 p, int depth = 7)
        {
            var result = 0.0;
            var weight = 1.0;
            // note: book code copies p, point3 is a struct and copied by value, we should
            // be fine messing directly with the parameter
            for (var i = 0; i < depth; ++i)
            {
                result += weight * Noise(p);
                weight *= 0.5;
                p = new Vector3(p.X * 2, p.Y * 2, p.Z * 2);
            }

            return Math.Abs(result);
        }

        private static void Permute(int[] arr)
        {
            for (var i = PointCount - 1; i > 0; i--)
            {
                // randomly swap two indices
                var target = Sampler.Instance.RandomInt(0, i);
                (arr[i], arr[target]) = (arr[target], arr[i]);
            }
        }

        // tri-linear interpolation to smooth noise results
        private static double TriLinearInterpolate(Vec3[,,] c, double u, double v, double w)
        {
            var result = 0.0;

            var uu = u * u * (3 - 2 * u);
            var vv = v * v * (3 - 2 * v);
            var ww = w * w * (3 - 2 * w);

            for (var i = 0; i < 2; ++i)
            for (var j = 0; j < 2; ++j)
            for (var k = 0; k < 2; ++k)
            {
                var weight = new Vec3(u - i, v - j, w - k);
                result += (i * uu + (1 - i) * (1 - u)) *
                          (j * vv + (1 - j) * (1 - v)) *
                          (k * ww + (1 - k) * (1 - w)) *
                          Vec3.Dot(c[i, j, k], weight);
            }

            return result;
        }
    }
}