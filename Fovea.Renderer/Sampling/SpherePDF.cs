using System;

namespace Fovea.Renderer.Sampling;

public readonly struct SpherePDF : IPDF
{
    public float Evaluate(Vector3 direction) => 1.0f / (4.0f * MathF.PI);
    public Vector3 Generate() => Sampler.Instance.RandomOnUnitSphere();
}