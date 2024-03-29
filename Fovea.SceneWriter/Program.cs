﻿// serialize demo and book scenes to json, mostly for scenes that do contain randomized elements

using System.Diagnostics;
using System.Text.Json;
using CommandLine;
using Fovea.Renderer.Parser.Json;
using Fovea.SceneWriter;

Parser.Default.ParseArguments<CommandLineArgs>(args)
    .WithParsed(opts =>
    {
        try
        {
            var sceneDescriptor = DemoSceneCreator.MakeScene(opts.SceneId);
            var jsonText = JsonSerializer.Serialize(sceneDescriptor, JsonParser.JsonOptions);
            File.WriteAllText(opts.OutputFile, jsonText);
            Debug.WriteLine($"wrote scene to {opts.OutputFile}");
        }
        catch (Exception err)
        {
            Debug.WriteLine($"something went wrong {err}");
        }
    });