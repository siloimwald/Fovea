using System.Collections.Generic;
using System.Linq;

namespace Fovea.Renderer.VectorMath.Transforms
{
    /// <summary>
    ///     a simple transformation approach. Keep a list of all individual transformation steps around. Each of these is
    ///     easy to invert and easy to compute the matrix for. to invert the whole matrix, apply the inverse individual
    ///     transformations in reverse
    /// </summary>
    public class Transformation
    {
        private readonly List<ISimpleTransform> _transforms = new();

        public Transformation Translate(double tx, double ty, double tz)
        {
            _transforms.Add(new Translation(tx, ty, tz));
            return this;
        }

        public Transformation Translate(Vec3 tVec)
        {
            return Translate(tVec.X, tVec.Y, tVec.Z);
        }

        public Transformation Scale(double sx, double sy, double sz)
        {
            _transforms.Add(new Scaling(sx, sy, sz));
            return this;
        }

        public Transformation Scale(Vec3 sVec)
        {
            return Scale(sVec.X, sVec.Y, sVec.Z);
        }

        public Transformation Rotate(double angle, Axis axis)
        {
            _transforms.Add(new Rotation(angle, axis));
            return this;
        }

        public Matrix4 GetMatrix()
        {
            return
                (_transforms as IEnumerable<ISimpleTransform>)
                .Reverse().Select(p => p.GetMatrix()).Aggregate(Matrix4.Identity(), (m0, m1) => m0 * m1);
        }

        public Matrix4 GetInverseMatrix()
        {
            return _transforms.Select(p => p.GetInverseMatrix()).Aggregate(Matrix4.Identity(), (m0, m1) => m0 * m1);
        }
    }
}