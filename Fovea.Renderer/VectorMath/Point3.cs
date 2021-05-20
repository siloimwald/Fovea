using System;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// in contrast to the books, also have a dedicated point3 class 
    /// </summary>
    public readonly struct Point3 : IEquatable<Point3>
    {
        public readonly float PX;
        public readonly float PY;
        public readonly float PZ;

        public Point3(float s = 0.0f)
        {
            PX = PY = PZ = s;
        }

        public Point3(float px, float py, float pz)
        {
            PX = px;
            PY = py;
            PZ = pz;
        }
        
        #region operators

        public float this[int index]
        {
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
        public static Point3 operator +(Point3 p, Vec3 v)
            => new(p.PX + v.X, p.PY + v.Y, p.PZ + v.Z);

        // point minus vector yields... point as well. This is just convenience instead of writing p+(-v)
        public static Point3 operator -(Point3 p, Vec3 v)
            => new(p.PX - v.X, p.PY - v.Y, p.PZ - v.Z);

        // point minus points yields the vector between both points
        public static Vec3 operator -(Point3 left, Point3 right)
            => new(left.PX - right.PX, left.PY - right.PY, left.PZ - right.PZ);

        #endregion

        public static Point3 Max(Point3 left, Point3 right)
        {
            return new(
                MathF.Max(left.PX, right.PX),
                MathF.Max(left.PY, right.PY),
                MathF.Max(left.PZ, right.PZ)
            );
        }
        
        public static Point3 Min(Point3 left, Point3 right)
        {
            return new(
                MathF.Min(left.PX, right.PX),
                MathF.Min(left.PY, right.PY),
                MathF.Min(left.PZ, right.PZ)
            );
        }
        
        
        // IEquatable interface, see Vec3
        
        public bool Equals(Point3 other) => (this - other).Length() < 1e-8f;
        public override bool Equals(object obj) => obj is Point3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(PX, PY, PZ);
        public static bool operator ==(Point3 left, Point3 right) => left.Equals(right);
        public static bool operator !=(Point3 left, Point3 right) => !(left == right);
    }
}