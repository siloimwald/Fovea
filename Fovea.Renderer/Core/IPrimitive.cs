using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    public interface IPrimitive
    {
        bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord);
        BoundingBox GetBoundingBox(double t0, double t1);

        /// <summary>
        /// pdf evaluated for the given direction
        /// </summary>
        /// <param name="origin">point looking from</param>
        /// <param name="direction">direction towards geometry</param>
        /// <returns></returns>
        double PdfValue(Point3 origin, Vec3 direction) => 0;

        /// <summary>
        /// generate a random direction towards the surface of the geometry
        /// </summary>
        /// <param name="origin">the point we're looking from</param>
        /// <returns></returns>
        Vec3 RandomDirection(Point3 origin) => Vec3.UnitX;
    }
}