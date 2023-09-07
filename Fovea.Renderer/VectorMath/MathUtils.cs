using System;
using System.Runtime.CompilerServices;

namespace Fovea.Renderer.VectorMath
{
    public static class MathUtils
    {
        public static float DegToRad(float degree)
        {
            return degree * MathF.PI / 180.0f;
        }

        /// <summary>general quadratics solver, from pbrt book with claimed higher numerical stability</summary>
        /// <returns>true if there was at least one solution</returns>
        public static bool SolveQuadratic(float a, float b, float c, ref float t0, ref float t1)
        {
            var disc = b * b - 4 * a * c;
            if (disc < 0)
                return false;

            var discRoot = MathF.Sqrt(disc);
            var q = b < 0 ? -0.5f * (b - discRoot) : -0.5f * (b + discRoot);
            t0 = q / a;
            t1 = c / q;
            if (t0 > t1)
                (t1, t0) = (t0, t1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampF(float v, float min, float max)
        {
            return MathF.Max(MathF.Min(v, max), min);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T left, ref T right)
        {
            (left, right) = (right, left);
        }
    }
}