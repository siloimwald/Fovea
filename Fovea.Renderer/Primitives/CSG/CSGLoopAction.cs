using System;

namespace Fovea.Renderer.Primitives.CSG
{

    [Flags]
    public enum CSGLoopAction
    {
        /// <summary>
        /// not at all
        /// </summary>
        ReturnMiss = 1,
        /// <summary>
        /// return intersection with A/Left if closer
        /// </summary>
        ReturnAIfCloser = 2,
        /// <summary>
        /// return intersection with A/Left if farther
        /// </summary>
        ReturnAIfFarther = 4,
        /// <summary>
        /// return intersection with A/Left
        /// </summary>
        ReturnA = 8,
        /// <summary>
        /// return intersection with B/right
        /// </summary>
        ReturnB = 16,
        /// <summary>
        /// return intersection with B/Right if it closer
        /// </summary>
        ReturnBIfCloser = 32,
        /// <summary>
        /// return intersection with B if farther
        /// </summary>
        ReturnBIfFarther = 64,
        
        /// <summary>
        /// return intersection with B/Right, flip normal
        /// </summary>
        FlipB = 128,
        
        /// <summary>
        /// continue with next intersection of A/Left
        /// </summary>
        AdvanceAAndLoop = 256,
        /// <summary>
        /// continue with next intersection of B/Right
        /// </summary>
        AdvanceBAndLoop = 512
    }
}