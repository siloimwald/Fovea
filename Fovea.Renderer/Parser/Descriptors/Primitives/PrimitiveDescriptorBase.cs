using System.Collections.Generic;
using System.Text.Json.Serialization;
using Fovea.Renderer.Core;
using Microsoft.Extensions.Logging;

namespace Fovea.Renderer.Parser.Descriptors.Primitives;

public abstract class PrimitiveDescriptorBase
{
    private static readonly ILogger<PrimitiveDescriptorBase> Log = Logging.GetLogger<PrimitiveDescriptorBase>();
    
    [JsonPropertyName("material")]
    public string MaterialReference { get; init; } = string.Empty;

    /// <summary>
    /// allow importance list to refer to this primitive
    /// </summary>
    public string Id { get; init; } = string.Empty;
    
    protected IMaterial GetMaterial(Dictionary<string, IMaterial> materials)
    {
        return materials.GetValueOrDefault(MaterialReference, null);
    }
}