using System;
using static System.MathF;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// 3D Vector
    /// </summary>
    public readonly struct Vec3
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        /// <summary>
        /// creates a new 3D vector with all components initialized to the same value
        /// </summary>
        /// <param name="s">scalar value to be used for all components</param>
        public Vec3(float s = 0.0f)
        {
            X = Y = Z = s;
        }

        /// <summary>
        /// creates a new 3D vector with the given components 
        /// </summary>
        /// <param name="x">x component</param>
        /// <param name="y">y component</param>
        /// <param name="z">z component</param>
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region operators

        // vector plus vector
        public static Vec3 operator +(Vec3 left, Vec3 right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        // vector minus vector
        public static Vec3 operator -(Vec3 left, Vec3 right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - left.Z);

        // scalar multiplication
        public static Vec3 operator *(Vec3 vec, float scalar)
            => new(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);

        // unary minus
        public static Vec3 operator -(Vec3 v)
            => new(-v.X, -v.Y, -v.Z);
        
        #endregion

        /// <summary>
        /// length or magnitude of vector
        /// </summary>
        /// <returns></returns>
        public float Length() => Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        /// squared length of vector
        /// </summary>
        public float LengthSquared() => X * X + Y * Y + Z * Z;

        /// <summary>
        /// dot product of both vectors
        /// </summary>
        public static float Dot(Vec3 left, Vec3 right)
            => left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        
        /// <summary>
        /// returns the vector normalized, with length 1
        /// </summary>
        /// <param name="v">3d vector</param>
        /// <returns></returns>
        public static Vec3 Normalize(Vec3 v)
        {
            var oneOverLen = 1.0f / v.Length();
            return new Vec3(v.X * oneOverLen, v.Y * oneOverLen, v.Z * oneOverLen);
        }
    }
}