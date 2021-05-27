namespace Fovea.Renderer.VectorMath.Transforms
{
    public readonly struct Translation : ISimpleTransform
    {
        public Translation(double tx, double ty, double tz)
        {
            TX = tx;
            TY = ty;
            TZ = tz;
        }

        public readonly double TX;
        public readonly double TY;
        public readonly double TZ;

        public Matrix4 GetMatrix() => Matrix4.GetTranslation(this);
        public Matrix4 GetInverseMatrix() => Matrix4.GetTranslation(new Translation(-TX, -TY, -TZ));
    }
}