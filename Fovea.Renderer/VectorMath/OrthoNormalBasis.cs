using System;

namespace Fovea.Renderer.VectorMath
{
    public class OrthoNormalBasis
    {
        public readonly Vec3 UAxis;
        public readonly Vec3 VAxis;
        public readonly Vec3 WAxis;
        
        public OrthoNormalBasis(Vec3 w)
        {
            WAxis = Vec3.Normalize(w);
            var a = Math.Abs(WAxis.X) > 0.9 ? Vec3.UnitY : Vec3.UnitX;
            VAxis = Vec3.Normalize(Vec3.Cross(WAxis, a));
            UAxis = Vec3.Cross(WAxis, VAxis);
        }

        public Vec3 Local(double a, double b, double c) => UAxis * a + VAxis * b + WAxis * c;
        public Vec3 Local(Vec3 v) => UAxis * v.X + VAxis * v.Y + WAxis * v.Z;
    }
}