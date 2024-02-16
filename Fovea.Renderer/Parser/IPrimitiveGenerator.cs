using System.Collections.Generic;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.Parser;

public interface IPrimitiveGenerator
{
    void Generate(IDictionary<string, IMaterial> materials, List<IPrimitive> existingPrimitives);
}