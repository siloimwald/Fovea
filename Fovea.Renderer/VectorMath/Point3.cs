using System;
using System.Runtime.CompilerServices;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// in contrast to the books, also have a dedicated point3 class 
    /// </summary>
    public readonly struct Point3 : IEquatable<Point3>
    {
        public readonly double PX;
        public readonly double PY;
        public readonly double PZ;

        public Point3(double s = 0.0)
        {
            PX = PY = PZ = s;
        }

        public Point3(double px, double py, double pz)
        {
            PX = px;
            PY = py;
            PZ = pz;
        }

        #region operators

        public double this[int index]
        {
            // looks ugly, but is only used during bounding box intersection and bvh construction
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index switch
                {
                    0 => PX,
                    1 => PY,
                    2 => PZ,
                    _ => throw new ArgumentException("invalid index", nameof(index))
                };
            }
        }

        // point plus vectors yields (translated) new point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 operator +(Point3 p, Vec3 v)
            => new(p.PX + v.X, p.PY + v.Y, p.PZ + v.Z);

        // point minus vector yields... point as well. This is just convenience instead of writing p+(-v)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 operator -(Point3 p, Vec3 v)
            => new(p.PX - v.X, p.PY - v.Y, p.PZ - v.Z);

        // point minus points yields the vector between both points
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 operator -(Point3 left, Point3 right)
            => new(left.PX - right.PX, left.PY - right.PY, left.PZ - right.PZ);

        #endregion

        public static Point3 Max(Point3 left, Point3 right)
        {
            return new(
                Math.Max(left.PX, right.PX),
                Math.Max(left.PY, right.PY),
                Math.Max(left.PZ, right.PZ)
            );
        }

        public static Point3 Min(Point3 left, Point3 right)
        {
            return new(
                Math.Min(left.PX, right.PX),
                Math.Min(left.PY, right.PY),
                Math.Min(left.PZ, right.PZ)
            );
        }

        public override string ToString() => $"<{PX}, {PY}, {PZ}>";

        // IEquatable interface, see Vec3

        public bool Equals(Point3 other) => (this - other).Length() < 1e-8;
        public override bool Equals(object obj) => obj is Point3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(PX, PY, PZ);
        public static bool operator ==(Point3 left, Point3 right) => left.Equals(right);
        public static bool operator !=(Point3 left, Point3 right) => !(left == right);
    }
}