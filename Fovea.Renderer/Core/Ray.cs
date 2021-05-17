using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    /// <summary>
    /// a ray in 3d space starting at some origin and pointing at some direction.
    /// This is way too big for a struct by any guidelines, but is allocated all over the place many many times
    /// </summary>
    public struct Ray
    {
        public Point3 Origin;
        public Vec3 Direction;

        public Ray(Point3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        /// <summary>
        /// returns the point where this is ray is pointing to for a given
        /// ray parameter t
        /// </summary>
        /// <param name="t">ray parameter</param>
        /// <returns></returns>
        public Point3 PointsAt(float t) => Origin + Direction * t;
    }
}