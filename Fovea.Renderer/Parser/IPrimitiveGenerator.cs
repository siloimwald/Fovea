using System.Collections.Generic;
using Fovea.Renderer.Core;
using Fovea.Renderer.Parser.Yaml;

namespace Fovea.Renderer.Parser;

public interface IPrimitiveGenerator
{
    List<IPrimitive> Generate(IDictionary<string, IMaterial> materials, ParserContext context);
}