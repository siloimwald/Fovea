﻿using System.IO;
using Fovea.Renderer.Materials;
using Fovea.Renderer.Materials.Texture;
using Fovea.Renderer.Parser.Json;

namespace Fovea.Renderer.Parser.Descriptors.Textures;

// a collection of parser entities/descriptors that map to textures/colors
// NOTE: most of these could directly work with the actual classes...

public class ImageTextureDescriptor : ITextureGenerator
{
    public string FileName { get; init; }

    public ITexture Generate(ParserContext context) => new ImageTexture(Path.Combine(context.SceneFileLocation, FileName));
}