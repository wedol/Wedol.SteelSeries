#!/usr/bin/dotnet run

#:package Newtonsoft.Json@13.0.4

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Get the directory where the script is located
var scriptDir = Environment.CurrentDirectory;
var swaggerPath = Path.Combine(scriptDir, "swagger.json");
var outputPath = Path.Combine(scriptDir, "swagger.processed.json");

Console.WriteLine($"Reading swagger.json from: {swaggerPath}");

// Read the swagger.json file
var swaggerJson = File.ReadAllText(swaggerPath);
var swagger = JObject.Parse(swaggerJson);

// Get the paths object
var paths = swagger["paths"] as JObject;
if (paths == null)
{
    Console.WriteLine("No paths found in swagger.json");
    return;
}

int operationCount = 0;

// Iterate through all paths
foreach (var pathProperty in paths.Properties())
{
    var path = pathProperty.Name;
    var pathItem = pathProperty.Value as JObject;

    if (pathItem == null) continue;

    // Process each HTTP method (get, post, put, delete, etc.)
    foreach (var operationProperty in pathItem.Properties())
    {
        var method = operationProperty.Name.ToLower();

        // Skip non-HTTP method properties
        if (!IsHttpMethod(method)) continue;

        var operation = operationProperty.Value as JObject;
        if (operation == null) continue;

        // Generate operationId if it doesn't exist
        if (operation["operationId"] == null)
        {
            var operationId = GenerateOperationId(path, method);
            operation["operationId"] = operationId;
            operationCount++;
            Console.WriteLine($"Added operationId: {operationId}");
        }
    }
}

// Write the processed swagger to a new file
File.WriteAllText(outputPath, swagger.ToString(Formatting.Indented));
Console.WriteLine($"\nProcessed {operationCount} operations");
Console.WriteLine($"Output written to: {outputPath}");

// Helper function to check if a property is an HTTP method
bool IsHttpMethod(string name)
{
    return name == "get" || name == "post" || name == "put" ||
           name == "delete" || name == "patch" || name == "options" ||
           name == "head" || name == "trace";
}

// Helper function to generate operationId from path and method
string GenerateOperationId(string path, string method)
{
    // Remove leading slash
    if (path.StartsWith("/"))
        path = path.Substring(1);

    // Split path into segments
    var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

    var sb = new StringBuilder();

    // Add HTTP method as prefix
    sb.Append(ToPascalCase(method));

    // Process each segment
    foreach (var segment in segments)
    {
        // Check if segment is a parameter (e.g., {id})
        if (segment.StartsWith("{") && segment.EndsWith("}"))
        {
            // Extract parameter name and add "By" prefix
            var paramName = segment.Trim('{', '}');
            sb.Append("By");
            sb.Append(ToPascalCase(paramName));
        }
        else
        {
            // Regular path segment
            sb.Append(ToPascalCase(segment));
        }
    }

    return sb.ToString();
}

// Helper function to convert string to PascalCase
string ToPascalCase(string input)
{
    if (string.IsNullOrEmpty(input))
        return input;

    // Remove special characters and split by common delimiters
    var words = Regex.Split(input, @"[^a-zA-Z0-9]+")
        .Where(w => !string.IsNullOrEmpty(w))
        .ToArray();

    var sb = new StringBuilder();
    foreach (var word in words)
    {
        if (word.Length > 0)
        {
            sb.Append(char.ToUpper(word[0]));
            if (word.Length > 1)
                sb.Append(word.Substring(1).ToLower());
        }
    }

    return sb.ToString();
}
