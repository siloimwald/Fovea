using System;
using Fovea.Renderer.Core;

namespace Fovea.CmdLine
{
    class Program
    {
        static void Main(string[] args)
        {
            var renderer = new Raytracer();
            renderer.Render();
        }
    }
}