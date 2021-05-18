using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Viewing
{
    public class PerspectiveCamera
    {
        private readonly Point3 _origin;
        private readonly Point3 _lowerLeft;
        private readonly Vec3 _vertical;
        private readonly Vec3 _horizontal;

        public PerspectiveCamera(Orientation orientation, float aspectRatio, float verticalFieldOfView)
        {
            var theta = MathUtils.DegToRad(verticalFieldOfView);
            var h = MathF.Tan(theta / 2.0f);
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var wAxis = Vec3.Normalize(orientation.LookFrom - orientation.LookAt);
            var uAxis = Vec3.Normalize(Vec3.Cross(orientation.UpDirection, wAxis));
            var vAxis = Vec3.Cross(wAxis, uAxis);

            _horizontal = uAxis * viewportWidth;
            _vertical = vAxis * viewportHeight;

            _origin = orientation.LookFrom;
            _lowerLeft = _origin - _horizontal * 0.5f - _vertical * 0.5f - wAxis;
        }

        public Ray ShootRay(float s, float t)
        {
            return new(_origin, _lowerLeft + _horizontal * s + _vertical * t - _origin);
        }
    }
}