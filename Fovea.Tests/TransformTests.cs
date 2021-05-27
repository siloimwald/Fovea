using Fovea.Renderer.VectorMath;
using Fovea.Renderer.VectorMath.Transforms;
using Xunit;

namespace Fovea.Tests
{
    public class TransformTests
    {
        [Fact]
        public void TestTranslation()
        {
            var m = new Transformation().Translate(2, -3, 0).GetMatrix();
            var p = new Point3(5, -3, 0.2);
            
            var transformedPoint = m * p;
            Assert.Equal(new Point3(7, -6, 0.2), transformedPoint);

            var v = new Vec3(1, -1, -3);
            var transformedVector = m * v;
            Assert.Equal(v, transformedVector);
        }

        [Fact]
        public void TestScaling()
        {
            var m = new Transformation().Scale(0.5, 2, -0.5).GetMatrix();
            var p = new Point3(2, -4, 10);

            var transformedPoint = m * p;
            Assert.Equal(new Point3(1, -8, -5), transformedPoint);

            var v = new Vec3(4, -2, 22);
            var transformedVector = m * v;
            Assert.Equal(new Vec3(2, -4, -11), transformedVector);
        }

        [Fact]
        public void TestRotateX()
        {
            var m0 = new Transformation().Rotate(90, Axis.X).GetMatrix();
            var p = new Point3(0, 0, 1);
            Assert.Equal(new Point3(0, -1, 0), m0 * p);
            var m1 = new Transformation().Rotate(-90, Axis.X).GetMatrix();
            Assert.Equal(new Point3(0, 1, 0), m1 * p);
        }

        [Fact]
        public void TestRotateY()
        {
            var m0 = new Transformation().Rotate(90, Axis.Y).GetMatrix();
            var p = new Point3(1, 0, 0);
            Assert.Equal(new Point3(0, 0, -1), m0 * p);
            var m1 = new Transformation().Rotate(-90, Axis.Y).GetMatrix();
            Assert.Equal(new Point3(0, 0, 1), m1 * p);
        }

        [Fact]
        public void TestRotateZ()
        {
            var m0 = new Transformation().Rotate(90, Axis.Z).GetMatrix();
            var p = new Point3(0, 1, 0);
            Assert.Equal(new Point3(-1, 0, 0), m0 * p);
            var m1 = new Transformation().Rotate(-90, Axis.Z).GetMatrix();
            Assert.Equal(new Point3(1, 0, 0), m1 * p);
        }

        [Fact]
        public void TestCompoundTransform()
        {
            // play nice and use the 'common' TRS order
            
            var transform = new Transformation()
                .Scale(2, 1, 2) // 10,2,-4
                .Rotate(90, Axis.X) // 10, 4, 2 
                .Translate(100, 100, -100);
            
            var forward = transform.GetMatrix();
            var p = new Point3(5, 2, -2);
            var transformedPoint = forward * p;
            Assert.Equal(new Point3(110, 104, -98), transformedPoint);
            
            // invert it, get old point back
            var inverse = transform.GetInverseMatrix();
            Assert.Equal(p, inverse * transformedPoint);
        }
        
        [Fact]
        public void TestLargerCompoundTransform()
        {
            // doing a bunch of random transformations
            var transform = new Transformation()
                .Translate(3, 2, -3)
                .Scale(2, -3, 2)
                .Rotate(30, Axis.Y)
                .Translate(20, -10, 2)
                .Scale(1, 0.5, 0.5)
                .Rotate(-60, Axis.Z);

            // reverting these piecewise should yield the inverse matrix
            var forward = transform.GetMatrix();
            var inverse = transform.GetInverseMatrix();
            var shouldBeI = forward * inverse;
            Assert.True(shouldBeI.IsIdentity());
            Assert.True((inverse*forward).IsIdentity());
        }
    }
}