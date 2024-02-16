namespace Fovea.Renderer.Parser;

// a collection of parser entities/descriptors that map to materials

public interface IMaterialGenerator;

public abstract class MaterialDescriptorBase
{
    /// <summary>
    /// string reference to the texture map within the yaml file
    /// </summary>
    public string Texture { get; init; } = string.Empty;
}

public class MatteDescriptor : MaterialDescriptorBase, IMaterialGenerator;


public class MetalDescriptor : MaterialDescriptorBase, IMaterialGenerator
{
    public float Fuzzy { get; init; }
}

