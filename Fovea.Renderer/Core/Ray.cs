using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    /// <summary>
    ///     a ray in 3d space starting at some origin and pointing at some direction. This is way too big for a struct by
    ///     any guidelines, but is allocated all over the place many many times
    /// </summary>
    public readonly struct Ray
    {
        public readonly Point3 Origin;
        public readonly Vec3 Direction;
        public readonly double Time;
        public readonly Vec3 InverseDirection;

        public Ray(Vector3 origin, Vector3 direction, double time = 0.0)
        {
            Origin = origin.AsPoint3();
            Direction = direction.AsVec3();
            Time = time;
            InverseDirection = new Vec3(1.0 / Direction.X, 1.0 / Direction.Y, 1.0 / Direction.Z);
        }
        
        public Ray(Point3 origin, Vec3 direction, double time = 0.0)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
            InverseDirection = new Vec3(1.0 / Direction.X, 1.0 / Direction.Y, 1.0 / Direction.Z);
        }

        /// <summary>returns the point where this is ray is pointing to for a given ray parameter t</summary>
        /// <param name="t">ray parameter</param>
        /// <returns></returns>
        public Point3 PointsAt(double t)
        {
            return Origin + Direction * t;
        }
    }
}