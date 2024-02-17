namespace Fovea.Renderer.Parser.Yaml;

/// <summary>
/// global context properties while parsing
/// </summary>
public class ParserContext
{
    /// <summary>
    /// absolute path to scene file, so texture files and meshes can be resolved through relative paths
    /// </summary>
    public string SceneFileLocation { get; set; }
}