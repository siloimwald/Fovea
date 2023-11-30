namespace Fovea.Renderer.Core;

/// <summary>keeps track of various bits of an intersection.</summary>
public struct HitRecord
{
    /// <summary>point where we've hit the object</summary>
    public Vector3 HitPoint;

    /// <summary>the ray parameter for the closest intersection</summary>
    public float RayT;

    /// <summary>
    ///     normal at intersection. the correction towards outward facing normal is done just before shading so this might
    ///     point into any direction prior to that
    /// </summary>
    public Vector3 Normal;

    /// <summary>whether we've hit the front face of the primitive (normal points towards ray)</summary>
    public bool IsFrontFace;

    /// <summary>ensure the normal points towards the ray and flip if necessary</summary>
    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        IsFrontFace = Vector3.Dot(ray.Direction, outwardNormal) < 0;
        Normal = IsFrontFace ? outwardNormal : -outwardNormal;
    }

    /// <summary>material at intersection</summary>
    public IMaterial Material { get; set; }

    /// <summary>u texture coordinate</summary>
    public float TextureU;

    /// <summary>v texture coordinate</summary>
    public float TextureV;
}