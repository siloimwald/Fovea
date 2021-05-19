using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Viewing
{
    public class PerspectiveCamera
    {
        private readonly Point3 _origin;
        private readonly Point3 _lowerLeft;
        private readonly Vec3 _vertical;
        private readonly Vec3 _horizontal;
        private readonly float _lensRadius;
        private readonly Vec3 _uAxis;
        private readonly Vec3 _vAxis;

        public PerspectiveCamera(Orientation orientation,
            float aspectRatio,
            float verticalFieldOfView,
            float aperture,
            float focusDistance)
        {
            var theta = MathUtils.DegToRad(verticalFieldOfView);
            var h = MathF.Tan(theta / 2.0f);
            var viewportHeight = 2.0f * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var wAxis = Vec3.Normalize(orientation.LookFrom - orientation.LookAt);
            _uAxis = Vec3.Normalize(Vec3.Cross(orientation.UpDirection, wAxis));
            _vAxis = Vec3.Cross(wAxis, _uAxis);

            _horizontal = _uAxis * viewportWidth * focusDistance;
            _vertical = _vAxis * viewportHeight * focusDistance;

            _origin = orientation.LookFrom;
            _lowerLeft = _origin - _horizontal * 0.5f - _vertical * 0.5f - wAxis * focusDistance;
            _lensRadius = aperture / 2.0f;
        }

        public Ray ShootRay(float s, float t)
        {
            var (px, py) = Sampler.Instance.RandomOnUnitDisk();
            var offset = _uAxis * (px * _lensRadius) + _vAxis * (py * _lensRadius);
            return new Ray(_origin + offset,
                _lowerLeft + _horizontal * s + _vertical * t - _origin - offset);
        }
    }
}