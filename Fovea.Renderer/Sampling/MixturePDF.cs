namespace Fovea.Renderer.Sampling;

public class MixturePDF : IPDF
{
    private readonly IPDF _left;
    private readonly IPDF _right;

    public MixturePDF(IPDF left, IPDF right)
    {
        _left = left;
        _right = right;
    }

    public float Evaluate(Vector3 direction)
    {
        return 0.5f * _left.Evaluate(direction) + 0.5f * _right.Evaluate(direction);
    }

    public Vector3 Generate()
    {
        return Sampler.Instance.Random01() < 0.5 ? _left.Generate() : _right.Generate();
    }
}