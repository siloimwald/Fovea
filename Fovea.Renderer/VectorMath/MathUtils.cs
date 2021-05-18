using System;

namespace Fovea.Renderer.VectorMath
{
    public static class MathUtils
    {
        public static float DegToRad(float degree) => degree * MathF.PI / 180.0f;
    }
}