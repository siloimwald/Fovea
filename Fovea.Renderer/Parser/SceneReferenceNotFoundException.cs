using System;

namespace Fovea.Renderer.Parser;

public class SceneReferenceNotFoundException(string message) : Exception(message);