using System;
using System.Runtime.CompilerServices;

namespace Fovea.Renderer.VectorMath
{
    public static class MathUtils
    {
        public static double DegToRad(double degree) => degree * Math.PI / 180.0;
        
        /// <summary>
        /// general quadratics solver, from pbrt book with claimed higher numerical stability
        /// </summary>
        /// <returns>true if there was at least one solution</returns>
        public static bool SolveQuadratic(double a, double b, double c, ref double t0, ref double t1)
        {   
            var disc = b * b - 4 * a * c;
            if (disc < 0)
                return false;

            var discRoot = Math.Sqrt(disc);
            var q = b < 0 ? -0.5 * (b - discRoot) : -0.5 * (b + discRoot);
            t0 = q / a;
            t1 = c / q;
            if (t0 > t1)
                (t1, t0) = (t0, t1);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T left, ref T right)
        {
            var tmp = left;
            left = right;
            right = tmp;
        }
    }
}