using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Viewing
{
    public class PerspectiveCamera
    {
        // private readonly Vec3 _horizontal;
        // private readonly float _lensRadius;
        // private readonly Point3 _lowerLeft;
        // private readonly Point3 _origin;
        private readonly float _time0;
        private readonly float _time1;
        // private readonly Vec3 _uAxis;
        // private readonly Vec3 _vAxis;
        // private readonly Vec3 _vertical;

        private readonly Matrix4x4 _inverseView;
        private readonly Matrix4x4 _inverseProjection;
        private readonly Vector3 _center;
        
        public PerspectiveCamera(Orientation orientation,
            float aspectRatio,
            float verticalFieldOfView,
            float aperture,
            float focusDistance,
            float time0 = 0,
            float time1 = 0)
        {
            AspectRatio = aspectRatio;
            _time0 = time0;
            _time1 = time1;
            //
            // var h = Math.Tan(theta / 2.0);
            // var viewportHeight = 2.0 * h;
            // var viewportWidth = aspectRatio * viewportHeight;
            //
            // var wAxis = Vec3.Normalize(orientation.LookFrom - orientation.LookAt);
            // _uAxis = Vec3.Normalize(Vec3.Cross(orientation.UpDirection, wAxis));
            // _vAxis = Vec3.Cross(wAxis, _uAxis);
            //
            // _horizontal = _uAxis * viewportWidth * focusDistance;
            // _vertical = _vAxis * viewportHeight * focusDistance;
            //
            // _origin = orientation.LookFrom;
            // _lowerLeft = _origin - _horizontal * 0.5 - _vertical * 0.5 - wAxis * focusDistance;
            // _lensRadius = aperture / 2.0f;

            var theta = (float)MathUtils.DegToRad(verticalFieldOfView);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(theta, AspectRatio, focusDistance, 1000);
            _center = orientation.LookFrom;
            var viewMatrix = Matrix4x4.CreateLookAt(orientation.LookFrom, orientation.LookAt, orientation.UpDirection);
            var failedToInvert = Matrix4x4.Invert(viewMatrix, out _inverseView);
            failedToInvert &= Matrix4x4.Invert(projection, out _inverseProjection);
            if (!failedToInvert)
            {
                throw new Exception("failed to invert camera matrices");
            }
        }

        public float AspectRatio { get; }

        public Ray ShootRay(float s, float t)
        {
            var target = Vector4.Transform(new Vector4(s, t, 1.0f, 1.0f), _inverseProjection);
            var dir = Vector4.Transform((target / target.W), _inverseView);
            
            return new Ray(_center, new Vector3(dir.X, dir.Y, dir.Z) - _center, Sampler.Instance.Random(_time0, _time1));
            
            // var (px, py) = Sampler.Instance.RandomOnUnitDisk();
            // var offset = _uAxis * (px * _lensRadius) + _vAxis * (py * _lensRadius);
            // return new Ray(_origin + offset,
            //     _lowerLeft + _horizontal * s + _vertical * t - _origin - offset,
            //     Sampler.Instance.Random(_time0, _time1));
        }
    }
}