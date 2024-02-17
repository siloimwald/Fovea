using System.Collections.Generic;
using Fovea.Renderer.Core;

namespace Fovea.Renderer.Parser;

public interface IPrimitiveGenerator
{
    List<IPrimitive> Generate(IDictionary<string, IMaterial> materials);
}