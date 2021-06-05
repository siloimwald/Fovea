namespace Fovea.Renderer.VectorMath.Transforms
{
    public readonly struct Scaling : ISimpleTransform
    {
        public Scaling(double sx, double sy, double sz)
        {
            SX = sx;
            SY = sy;
            SZ = sz;
        }

        public readonly double SX;
        public readonly double SY;
        public readonly double SZ;

        public Matrix4 GetMatrix()
        {
            return Matrix4.GetScaling(this);
        }

        public Matrix4 GetInverseMatrix()
        {
            return Matrix4.GetScaling(new Scaling(1.0 / SX, 1.0 / SY, 1.0 / SZ));
        }
    }
}