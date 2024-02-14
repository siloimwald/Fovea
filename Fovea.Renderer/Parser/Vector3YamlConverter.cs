using System;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fovea.Renderer.Parser;

/// <summary>
/// the DotNetYaml Documentation is rather sparse...
/// This is an attempt to deserialize tagged lists of floats to Vector3
/// The tagging seems awful, but allows us to do Vector4 or others as well
/// </summary>
public class Vector3YamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(Vector3);
    }

    public object ReadYaml(IParser parser, Type type)
    {
        parser.Consume<SequenceStart>();
        var x = parser.Consume<Scalar>();
        var y = parser.Consume<Scalar>();
        var z = parser.Consume<Scalar>();
        parser.Consume<SequenceEnd>();
        // use invariant culture to treat decimal separators correctly
        return new Vector3(float.Parse(x.Value, CultureInfo.InvariantCulture),
            float.Parse(y.Value, CultureInfo.InvariantCulture),
            float.Parse(z.Value, CultureInfo.InvariantCulture));
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        throw new NotSupportedException("no serialization supported, yet");
    }
}