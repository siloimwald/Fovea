using System.Collections.Generic;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.Parser.Json;

/// <summary>
/// global context properties while parsing
/// </summary>
public class ParserContext
{
    /// <summary>
    /// absolute path to scene file, so texture files and meshes can be resolved through relative paths
    /// </summary>
    public string SceneFileLocation { get; set; }
    
    /// <summary>
    /// generated blueprints with null material
    /// </summary>
    public Dictionary<string, IPrimitive> Blueprints { get; set; }
}