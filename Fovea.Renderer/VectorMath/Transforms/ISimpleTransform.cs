
namespace Fovea.Renderer.VectorMath.Transforms
{
    public interface ISimpleTransform
    {
        Matrix4 GetMatrix();
        Matrix4 GetInverseMatrix();
    }
}