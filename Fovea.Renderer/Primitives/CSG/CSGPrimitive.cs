using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;

namespace Fovea.Renderer.Primitives.CSG
{
    /// <summary>
    /// from Andrew Kensler's Paper
    /// "Ray Tracing CSG Objects Using Single Hit Intersections"
    /// last received at http://xrt.wdfiles.com/local--files/doc%3Acsg/CSG.pdf
    /// A or Left and B or Right refer to the two primitives involved
    /// in a csg set operation, i.e. A n B or Left n Right
    /// </summary>
    public class CSGPrimitive : IPrimitive
    {
        // picking the names is somewhat arbitrary, if you read it as set operations this might make some sense
        private readonly IPrimitive _left;
        private readonly IPrimitive _right;
        private readonly CSGOperation _op;

        public CSGPrimitive(IPrimitive left, IPrimitive right, CSGOperation op)
        {
            _left = left;
            _right = right;
            _op = op;
        }

        public bool Hit(in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var hrLeft = new HitRecord();
            var hrRight = new HitRecord();

            var tMinLeft = tMin;
            var tMinRight = tMin;
            
            var stateLeft = Classify(_left, ray, tMinLeft, tMax, ref hrLeft);
            var stateRight = Classify(_right, ray, tMinRight, tMax, ref hrRight);

            var action = (CSGLoopAction) 0;
            
            do
            {
                action = ApplyActionTable(stateLeft, stateRight);
                
                if (action.HasFlag(CSGLoopAction.ReturnMiss))
                {
                    return false;
                }
                
                // NOTE: checking for <= and >= in both directions, otherwise
                // this breaks as an infinite loop for triangles which are in the same
                // plane and having the exact same hit. 
                // This might yield Enter, Enter for both, which breaks ReturnXIfFarther for Intersection
                
                if (action.HasFlag(CSGLoopAction.ReturnA)
                    || (action.HasFlag(CSGLoopAction.ReturnAIfCloser) && hrLeft.RayT <= hrRight.RayT)
                    || (action.HasFlag(CSGLoopAction.ReturnAIfFarther) && hrLeft.RayT >= hrRight.RayT))
                {
                    hitRecord = hrLeft;
                    return true;
                }

                if (action.HasFlag(CSGLoopAction.ReturnB)
                    || (action.HasFlag(CSGLoopAction.ReturnBIfCloser) && hrRight.RayT <= hrLeft.RayT)
                    || (action.HasFlag(CSGLoopAction.ReturnBIfFarther) && hrRight.RayT >= hrLeft.RayT))
                {
                    if (action.HasFlag(CSGLoopAction.FlipB))
                    {
                        // or toggle IsFrontFace? still not sure...
                        hrRight.Normal = -hrRight.Normal;
                    }

                    hitRecord = hrRight;
                    return true;
                }

                // avoid self intersection, otherwise we end up in an infinite loop
                // add some epsilon to next minLeft/minRight
                // could also check if we have twice Enter in a row?
                if (action.HasFlag(CSGLoopAction.AdvanceAAndLoop))
                {
                    tMinLeft = hrLeft.RayT + 1e-4;
                    stateLeft = Classify(_left, ray, tMinLeft, tMax, ref hrLeft);
                }
                else if (action.HasFlag(CSGLoopAction.AdvanceBAndLoop))
                {
                    tMinRight = hrRight.RayT + 1e-4; 
                    stateRight = Classify(_right, ray, tMinRight, tMax, ref hrRight);
                }

            } while (action != 0);
            
            return false;
        }

        private CSGLoopAction ApplyActionTable(CSGHitClassification left, CSGHitClassification right)
        {
            if (_op == CSGOperation.Union)
            {
                // first the easy cases, we miss either
                if (left == CSGHitClassification.Miss || right == CSGHitClassification.Miss)
                {
                    // missing both
                    if (left == CSGHitClassification.Miss && right == CSGHitClassification.Miss)
                        return CSGLoopAction.ReturnMiss;
                    return left == CSGHitClassification.Miss ? CSGLoopAction.ReturnB : CSGLoopAction.ReturnA;
                }
                
                if (left == CSGHitClassification.Enter && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.ReturnBIfCloser;
                
                if (left == CSGHitClassification.Enter && right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnBIfCloser | CSGLoopAction.AdvanceAAndLoop;
                
                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.AdvanceBAndLoop;
                
                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnAIfFarther | CSGLoopAction.ReturnBIfFarther;
            }

            if (_op == CSGOperation.Intersect)
            {
                // again, easy cases first
                if (left == CSGHitClassification.Miss || right == CSGHitClassification.Miss)
                    return CSGLoopAction.ReturnMiss;
                
                if (left == CSGHitClassification.Enter && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnAIfFarther | CSGLoopAction.ReturnBIfFarther;

                if (left == CSGHitClassification.Enter && right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.AdvanceBAndLoop;

                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnBIfCloser | CSGLoopAction.AdvanceAAndLoop;

                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.ReturnBIfCloser;
            }

            if (_op == CSGOperation.Difference)
            {
                if (left == CSGHitClassification.Miss)
                    return CSGLoopAction.ReturnMiss;

                if (right == CSGHitClassification.Miss)
                    return CSGLoopAction.ReturnA;

                if (left == CSGHitClassification.Enter && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.AdvanceBAndLoop;

                if (left == CSGHitClassification.Enter & right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnAIfFarther | CSGLoopAction.AdvanceAAndLoop;

                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Enter)
                    return CSGLoopAction.ReturnAIfCloser | CSGLoopAction.ReturnBIfCloser | CSGLoopAction.FlipB;

                if (left == CSGHitClassification.Exit && right == CSGHitClassification.Exit)
                    return CSGLoopAction.ReturnBIfCloser | CSGLoopAction.FlipB | CSGLoopAction.AdvanceAAndLoop;
            }
            
            return 0;
        }
        
        private static CSGHitClassification Classify(IPrimitive prim, in Ray ray, double tMin, double tMax, ref HitRecord hitRecord)
        {
            var hit = prim.Hit(ray, tMin, tMax, ref hitRecord);
            if (!hit)
                return CSGHitClassification.Miss;

            return hitRecord.IsFrontFace
                ? CSGHitClassification.Enter 
                : CSGHitClassification.Exit;
        }
        
        public BoundingBox GetBoundingBox()
        {
            return _op switch
            {
                CSGOperation.Union => BoundingBox.Union(_left.GetBoundingBox(), _right.GetBoundingBox()),
                CSGOperation.Intersect => BoundingBox.Intersect(_left.GetBoundingBox(), _right.GetBoundingBox()),
                _ => _left.GetBoundingBox() // we can't reduce the left one in any way, but right does not matter
            };
        }
    }
}