namespace Fovea.Renderer.Sampling;

public readonly struct MixturePDF(IPDF left, IPDF right) : IPDF
{
    public float Evaluate(Vector3 direction)
    {
        return 0.5f * left.Evaluate(direction) + 0.5f * right.Evaluate(direction);
    }

    public Vector3 Generate()
    {
        return Sampler.Instance.Random01() < 0.5 ? left.Generate() : right.Generate();
    }
}