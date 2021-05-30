namespace Fovea.Renderer.Primitives.CSG
{
    /// <summary>
    /// ray object interaction 
    /// </summary>
    public enum CSGHitClassification
    {
        /// <summary>
        /// ray origin outside
        /// </summary>
        Enter,

        /// <summary>
        /// ray origin inside
        /// </summary>
        Exit,

        /// <summary>
        /// no hit
        /// </summary>
        Miss
    }
}