using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Viewing
{
    public class PerspectiveCamera
    {
        private readonly double _time0;
        private readonly double _time1;
        private readonly Point3 _origin;
        private readonly Point3 _lowerLeft;
        private readonly Vec3 _vertical;
        private readonly Vec3 _horizontal;
        private readonly double _lensRadius;
        private readonly Vec3 _uAxis;
        private readonly Vec3 _vAxis;

        public PerspectiveCamera(Orientation orientation,
            double aspectRatio,
            double verticalFieldOfView,
            double aperture,
            double focusDistance,
            double time0 = 0,
            double time1 = 0)
        {
            _time0 = time0;
            _time1 = time1;
            
            var theta = MathUtils.DegToRad(verticalFieldOfView);
            var h = Math.Tan(theta / 2.0);
            var viewportHeight = 2.0 * h;
            var viewportWidth = aspectRatio * viewportHeight;

            var wAxis = Vec3.Normalize(orientation.LookFrom - orientation.LookAt);
            _uAxis = Vec3.Normalize(Vec3.Cross(orientation.UpDirection, wAxis));
            _vAxis = Vec3.Cross(wAxis, _uAxis);

            _horizontal = _uAxis * viewportWidth * focusDistance;
            _vertical = _vAxis * viewportHeight * focusDistance;

            _origin = orientation.LookFrom;
            _lowerLeft = _origin - _horizontal * 0.5 - _vertical * 0.5 - wAxis * focusDistance;
            _lensRadius = aperture / 2.0;
        }

        public Ray ShootRay(double s, double t)
        {
            var (px, py) = Sampler.Instance.RandomOnUnitDisk();
            var offset = _uAxis * (px * _lensRadius) + _vAxis * (py * _lensRadius);
            return new Ray(_origin + offset,
                _lowerLeft + _horizontal * s + _vertical * t - _origin - offset,
                Sampler.Instance.Random(_time0, _time1));
        }
    }
}