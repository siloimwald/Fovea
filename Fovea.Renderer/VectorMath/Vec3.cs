using System;
using static System.MathF;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// 3D Vector
    /// </summary>
    public readonly struct Vec3 : IEquatable<Vec3>
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
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        // scalar multiplication
        public static Vec3 operator *(Vec3 vec, float scalar)
            => new(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);

        // unary minus
        public static Vec3 operator -(Vec3 v)
            => new(-v.X, -v.Y, -v.Z);
        
        public float this[int index]
        {
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
        /// test whether this vector is close to the zero vector
        /// </summary>
        /// <returns></returns>
        public bool IsNearZero()
            => Abs(X) < 1e-8f && Abs(Y) < 1e-8f && Abs(Z) < 1e-8f;
        
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

        /// <summary>
        /// cross product
        /// </summary>
        /// <returns></returns>
        public static Vec3 Cross(Vec3 left, Vec3 right)
        {
            return new(left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        /// <summary>
        /// compute outgoing direction for a vector reflected across normal n
        /// </summary>
        /// <param name="w">incoming vector</param>
        /// <param name="n">surface normal</param>
        /// <returns></returns>
        public static Vec3 Reflect(Vec3 w, Vec3 n)
        {
            return w - n * Dot(w, n) * 2.0f;
        }

        /// <summary>
        /// refract incoming direction at surface normal with
        /// the given refraction index
        /// </summary>
        /// <param name="uv">incoming direction</param>
        /// <param name="normal">surface normal at intersection</param>
        /// <param name="etaIOverEtaN">eta i / eta n (refraction indices)</param>
        /// <returns></returns>
        public static Vec3 Refract(Vec3 uv, Vec3 normal, float etaIOverEtaN)
        {
            var cosTheta = Min(1.0f, Dot(-uv, normal));
            var dirOutPerpendicular = (uv + normal * cosTheta) * etaIOverEtaN;
            var dirOutParallel = normal * -Sqrt(Abs(1.0f - dirOutPerpendicular.LengthSquared()));
            return dirOutParallel + dirOutPerpendicular;
        }
        
        // IEquatable, almost exclusively used for unit tests. mostly to have
        // the 'fuzzy' equality
        
        public bool Equals(Vec3 other) => (this - other).Length() < 1e-8f;
        public override bool Equals(object obj) => obj is Vec3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        public static bool operator ==(Vec3 left, Vec3 right) => left.Equals(right);
        public static bool operator !=(Vec3 left, Vec3 right) => !(left == right);
    }
}