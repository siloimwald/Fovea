using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Core
{
    /// <summary>
    /// keeps track of various bits of an intersection
    /// </summary>
    public class HitRecord
    {
        /// <summary>
        /// point where we've hit the object
        /// </summary>
        public Point3 HitPoint;

        /// <summary>
        /// the ray parameter for this hit
        /// </summary>
        public float RayT;

        /// <summary>
        /// normal at intersection. the correction towards outward facing normal is done just before shading
        /// so this might point into any direction prior to that
        /// </summary>
        public Vec3 Normal;

        /// <summary>
        /// whether we've hit the front face of the primitive (normal points towards ray)
        /// </summary>
        public bool IsFrontFace;

        /// <summary>
        /// ensure the normal points towards the ray and flip if necessary
        /// </summary>
        public void ProcessNormal(Ray ray)
        {
            IsFrontFace = Vec3.Dot(ray.Direction, Normal) < 0;
            Normal = IsFrontFace ? Normal : -Normal;
        }
        
        /// <summary>
        /// material at intersection
        /// </summary>
        public IMaterial Material { get; set; }
    }
}