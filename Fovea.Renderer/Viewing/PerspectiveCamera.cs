using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser;
using Fovea.Renderer.Sampling;
using Fovea.Renderer.VectorMath;
using static System.MathF;

namespace Fovea.Renderer.Viewing;

// back to book 1 level for now...
public class PerspectiveCamera
{
    private readonly Vector3 _center; // camera projection center
    
    // camera frame basis vectors
    private readonly Vector3 _uAxis;
    private readonly Vector3 _vAxis;
    private readonly Vector3 _wAxis;
    // delta one pixel to the next
    private readonly Vector3 _pixelDeltaU;
    private readonly Vector3 _pixelDeltaV;
    // depth of field trickery
    private readonly Vector3 _defocusDiskU;
    private readonly Vector3 _defocusDiskV;
    private readonly bool _useDepthOfField;
    
    private readonly Vector3 _pixelZeroZero;
    private readonly float _oneOverSqrtSpp;
    
    public PerspectiveCamera(CameraDescriptor cameraDescriptor,
        RenderOptions renderOptions)

    {
        _center = cameraDescriptor.Orientation.LookFrom;
        var aspectRatio = renderOptions.ImageWidth / (float)renderOptions.ImageHeight;
        var theta = MathUtils.DegToRad(cameraDescriptor.FieldOfView);
        var h = Tan(theta / 2.0f);
        var viewportHeight = 2.0f * h * cameraDescriptor.FocusDistance;
        var viewportWidth = aspectRatio * viewportHeight;

        _oneOverSqrtSpp = 1.0f / Sqrt(Max(1, renderOptions.NumSamples));
        
        _wAxis = Vector3.Normalize(cameraDescriptor.Orientation.LookFrom - cameraDescriptor.Orientation.LookAt);
        _uAxis = Vector3.Normalize(Vector3.Cross(cameraDescriptor.Orientation.UpDirection, _wAxis));
        _vAxis = Vector3.Cross(_wAxis, _uAxis);

        // Calculate the vectors across the horizontal and down the vertical viewport edges.
        var viewportU = viewportWidth * _uAxis;
        var viewportV = viewportHeight * -_vAxis;

        _pixelDeltaU = viewportU / renderOptions.ImageWidth;
        _pixelDeltaV = viewportV / renderOptions.ImageHeight;

        var viewportUpperLeft = _center
                                - cameraDescriptor.FocusDistance * _wAxis
                                - viewportU / 2.0f
                                - viewportV / 2.0f;
        _pixelZeroZero = viewportUpperLeft + 0.5f * (_pixelDeltaU + _pixelDeltaV);
        
        // Calculate the camera defocus disk basis vectors.
        var defocusRadius = cameraDescriptor.FocusDistance
                            * Tan(MathUtils.DegToRad(cameraDescriptor.DefocusAngle/2.0f));
        _defocusDiskU = _uAxis * defocusRadius;
        _defocusDiskV = _vAxis * defocusRadius;
        _useDepthOfField = cameraDescriptor.DefocusAngle > 0;
    }

    public Ray ShootRay(int px, int py, int si, int sj)
    {
        var pixelCenter = _pixelZeroZero + _pixelDeltaU * px + _pixelDeltaV * py;
        var pixelSample = pixelCenter + PixelSampleSquare(si, sj);
        var origin = _useDepthOfField ? DefocusDiskSample() : _center;
        var dir = pixelSample - origin;
        var rayTime = Sampler.Instance.Random01();
        return new Ray(origin, dir, rayTime);
    }

    private Vector3 PixelSampleSquare(int si, int sj)
    {
        var r1 = -0.5f + _oneOverSqrtSpp * (si + Sampler.Instance.Random01());
        var r2 = -0.5f + _oneOverSqrtSpp * (sj + Sampler.Instance.Random01());
        return r1 * _pixelDeltaU + r2 * _pixelDeltaV;
    }

    private Vector3 DefocusDiskSample()
    {
        var (diskX, diskY) = Sampler.Instance.RandomOnUnitDisk();
        return _center + diskX * _defocusDiskU + diskY * _defocusDiskV;
    }
}