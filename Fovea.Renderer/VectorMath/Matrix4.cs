using System.Linq;
using Fovea.Renderer.VectorMath.Transforms;

using static System.Math;

namespace Fovea.Renderer.VectorMath
{
    /// <summary>
    /// reinventing the wheel with a 4x4 matrix, keep it simple for now
    /// </summary>
    public class Matrix4
    {
        // go a with a heap allocated array for now... we probably but instantiate these
        // left and right, but rather store them long term in instance primitives?!
        private readonly double[,] _matrix;

        public Matrix4()
        {
            _matrix = new double[4,4];
        }

        public static Matrix4 Identity()
        {
            var m = new Matrix4();
            m._matrix[0, 0] = m._matrix[1, 1] = m._matrix[2, 2] = m._matrix[3, 3] = 1.0;
            return m;
        }

        public static Matrix4 GetTranslation(Translation translation)
        {
            var m = new Matrix4();
            m._matrix[0, 0] = m._matrix[1, 1] = m._matrix[2, 2] = m._matrix[3, 3] = 1.0;
            m._matrix[0, 3] = translation.TX;
            m._matrix[1, 3] = translation.TY;
            m._matrix[2, 3] = translation.TZ;
            return m;
        }

        public static Matrix4 GetScaling(Scaling scaling)
        {
            var m = new Matrix4();
            m._matrix[0, 0] = scaling.SX;
            m._matrix[1, 1] = scaling.SY;
            m._matrix[2, 2] = scaling.SZ;
            m._matrix[3, 3] = 1.0;
            return m;
        }

        // matrix times matrix
        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            var m = new Matrix4();
            for (var resRow = 0; resRow < 4; ++resRow)
            {
                for (var resCol = 0 ; resCol < 4 ; ++resCol)
                {
                    m._matrix[resRow, resCol] =
                        left._matrix[resRow, 0] * right._matrix[0, resCol] +
                        left._matrix[resRow, 1] * right._matrix[1, resCol] +
                        left._matrix[resRow, 2] * right._matrix[2, resCol] +
                        left._matrix[resRow, 3] * right._matrix[3, resCol];
                }
            }

            return m;
        }

        // matrix times vector
        public static Vec3 operator *(Matrix4 m, Vec3 v)
        {
            return new(
                v.X * m._matrix[0, 0] + v.Y * m._matrix[0, 1] + v.Z * m._matrix[0, 2],
                v.X * m._matrix[1, 0] + v.Y * m._matrix[1, 1] + v.Z * m._matrix[1, 2],
                v.X * m._matrix[2, 0] + v.Y * m._matrix[2, 1] + v.Z * m._matrix[2, 2]
            );
        }
        
        // matrix times vector
        public static Point3 operator *(Matrix4 m, Point3 p)
        {
            return new(
                p.PX * m._matrix[0, 0] + p.PY * m._matrix[0, 1] + p.PZ * m._matrix[0, 2] + m._matrix[0,3],
                p.PX * m._matrix[1, 0] + p.PY * m._matrix[1, 1] + p.PZ * m._matrix[1, 2] + m._matrix[1,3],
                p.PX * m._matrix[2, 0] + p.PY * m._matrix[2, 1] + p.PZ * m._matrix[2, 2] + m._matrix[2,3]
            );
        }

        public static Matrix4 GetRotation(double angle, Axis axis)
        {
            var m = Identity();
            
            if (axis == Axis.X)
            {
                m._matrix[1, 1] = Cos(angle);
                m._matrix[1, 2] = -Sin(angle);
                m._matrix[2, 1] = Sin(angle);
                m._matrix[2, 2] = Cos(angle);
            }

            if (axis == Axis.Y)
            {
                m._matrix[0, 0] = Cos(angle);
                m._matrix[0, 2] = Sin(angle);
                m._matrix[2, 0] = -Sin(angle);
                m._matrix[2, 2] = Cos(angle);
            }

            if (axis != Axis.Z) return m;
            m._matrix[0, 0] = Cos(angle);
            m._matrix[0, 1] = -Sin(angle);
            m._matrix[1, 0] = Sin(angle);
            m._matrix[1, 1] = Cos(angle);

            return m;
        }

        /// <summary>
        /// tests whether this instance is the identity matrix. pretty much only used
        /// in unit tests
        /// </summary>
        /// <returns></returns>
        public bool IsIdentity()
        {
            const double eps = 1e-8;
            var diagonalOk = Abs(_matrix[0, 0] - 1) < eps
                             && Abs(_matrix[2, 2] - 1) < eps
                             && Abs(_matrix[3, 3] - 1) < eps;
            var restOk = Enumerable.Range(0, 4)
                .SelectMany(rowIndex => 
                    Enumerable.Range(0, 4)
                        .Select(columnIndex => (rowIndex, columnIndex)))
                .Where(index => index.rowIndex != index.columnIndex)
                .All(index => Abs(_matrix[index.rowIndex, index.columnIndex]) < eps);

            return diagonalOk && restOk;
        }
        
        public Vec3 TransformVectorTransposed(Vec3 normal)
        {
            return new(
                normal.X * _matrix[0, 0] + normal.Y * _matrix[1, 0] + normal.Z * _matrix[2, 0],
                normal.X * _matrix[0, 1] + normal.Y * _matrix[1, 1] + normal.Z * _matrix[2, 1],
                normal.X * _matrix[0, 2] + normal.Y * _matrix[1, 2] + normal.Z * _matrix[2, 2]
            );
        }
    }
}