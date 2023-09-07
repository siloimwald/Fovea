using System;
using System.Runtime.CompilerServices;
using static System.Math;

namespace Fovea.Renderer.VectorMath
{

    public static class Vector3Extensions
    {
        public static Vec3 AsVec3(this Vector3 v)
        {
            return new Vec3(v.X, v.Y, v.Z);
        }

        public static Vector3 AsVector3(this Vec3 v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }
        
    }
    
    /// <summary>3D Vector</summary>
    public readonly struct Vec3 : IEquatable<Vec3>
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        /// <summary>creates a new 3D vector with all components initialized to the same value</summary>
        /// <param name="s">scalar value to be used for all components</param>
        public Vec3(double s = 0.0)
        {
            X = Y = Z = s;
        }
        
        /// <summary>creates a new 3D vector with the given components</summary>
        /// <param name="x">x component</param>
        /// <param name="y">y component</param>
        /// <param name="z">z component</param>
        public Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region operators

        // vector plus vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator +(Vec3 left, Vec3 right)
        {
            return new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        // vector minus vector
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator -(Vec3 left, Vec3 right)
        {
            return new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        // scalar multiplication
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator *(Vec3 vec, double scalar)
        {
            return new(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        // unary minus
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator -(Vec3 v)
        {
            return new(-v.X, -v.Y, -v.Z);
        }

        public double this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new ArgumentException("invalid index", nameof(index))
                };
            }
        }

        public static Vec3 UnitX = new(1, 0, 0);
        public static Vec3 UnitY = new(0, 1, 0);
        public static Vec3 UnitZ = new(0, 0, 1);

        #endregion

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        /// <summary>length or magnitude of vector</summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length()
        {
            return Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>squared length of vector</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>test whether this vector is close to the zero vector</summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNearZero()
        {
            return Abs(X) < 1e-8 && Abs(Y) < 1e-8 && Abs(Z) < 1e-8;
        }

        /// <summary>dot product of both vectors</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(Vec3 left, Vec3 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        /// <summary>returns the vector normalized, with length 1</summary>
        /// <param name="v">3d vector</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Normalize(Vec3 v)
        {
            var oneOverLen = 1.0 / v.Length();
            return new Vec3(v.X * oneOverLen, v.Y * oneOverLen, v.Z * oneOverLen);
        }

        /// <summary>cross product</summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Cross(Vec3 left, Vec3 right)
        {
            return new(left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        /// <summary>compute outgoing direction for a vector reflected across normal n</summary>
        /// <param name="w">incoming vector</param>
        /// <param name="n">surface normal</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Reflect(Vec3 w, Vec3 n)
        {
            return w - n * Dot(w, n) * 2.0;
        }

        /// <summary>refract incoming direction at surface normal with the given refraction index</summary>
        /// <param name="uv">incoming direction</param>
        /// <param name="normal">surface normal at intersection</param>
        /// <param name="etaIOverEtaN">eta i / eta n (refraction indices)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Refract(Vec3 uv, Vec3 normal, double etaIOverEtaN)
        {
            var cosTheta = Min(1.0, Dot(-uv, normal));
            var dirOutPerpendicular = (uv + normal * cosTheta) * etaIOverEtaN;
            var dirOutParallel = normal * -Sqrt(Abs(1.0 - dirOutPerpendicular.LengthSquared()));
            return dirOutParallel + dirOutPerpendicular;
        }

        // IEquatable, almost exclusively used for unit tests. mostly to have
        // the 'fuzzy' equality

        public bool Equals(Vec3 other)
        {
            return (this - other).Length() < 1e-8;
        }

        public override bool Equals(object obj)
        {
            return obj is Vec3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public static bool operator ==(Vec3 left, Vec3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec3 left, Vec3 right)
        {
            return !(left == right);
        }
    }
}