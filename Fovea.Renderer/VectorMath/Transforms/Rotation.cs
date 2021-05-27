namespace Fovea.Renderer.VectorMath.Transforms
{
    public readonly struct Rotation : ISimpleTransform
    {
        // in rad
        public readonly double Angle;
        public readonly Axis Axis;

        public Rotation(double angle, Axis axis)
        {
            Angle = MathUtils.DegToRad(angle);
            Axis = axis;
        }

        public Matrix4 GetMatrix() => Matrix4.GetRotation(Angle, Axis);
        public Matrix4 GetInverseMatrix() => Matrix4.GetRotation(-Angle, Axis);

    }
}