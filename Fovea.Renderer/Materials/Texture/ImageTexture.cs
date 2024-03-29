using System;
using Fovea.Renderer.Core;
using Fovea.Renderer.VectorMath;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Fovea.Renderer.Materials.Texture;

public class ImageTexture : ITexture, IDisposable
{
    private readonly Image<RgbaVector> _imageBuffer;
    private static readonly ILogger<ImageTexture> Log = Logging.GetLogger<ImageTexture>();
    
    public ImageTexture(string fileName)
    {
        try
        {
            Log.LogDebug("Reading {textureFileName}", fileName);
            _imageBuffer = Image.Load<RgbaVector>(fileName);
        }
        catch
        {
            Log.LogWarning("Failed to read {textureFileName}", fileName);
        }
    }

    public RGBColor Value(float u, float v, Vector3 p)
    {
        if (_imageBuffer == null)
            return new RGBColor(0, 1, 1);

        var texU = MathUtils.ClampF(u, 0.0f, 1.0f);
        var texV = MathUtils.ClampF(v, 0.0f, 1.0f);
        var px = (int) (texU * (_imageBuffer.Width - 1));
        var py = (int) (texV * (_imageBuffer.Height - 1));
        return _imageBuffer[px, _imageBuffer.Height - py - 1].FromRgbaVector();
    }

    // public void Dispose()
    // {
    //     _imageBuffer?.Dispose();
    // }
    public void Dispose()
    {
        _imageBuffer?.Dispose();
    }
}