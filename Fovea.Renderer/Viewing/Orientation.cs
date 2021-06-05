using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Viewing
{
    /// <summary>camera orientation</summary>
    public class Orientation
    {
        public Point3 LookFrom { get; set; }
        public Point3 LookAt { get; set; }
        public Vec3 UpDirection { get; set; }
    }
}