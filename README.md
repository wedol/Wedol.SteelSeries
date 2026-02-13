# Wedol.SteelSeries.Sonar

A .NET Standard 2.1 client for the SteelSeries Sonar local API, generated from `swagger.json` using NSwag.

Tested with SteelSeries GG software version 10.4.1.0. The `swagger.json` file was extracted from `SteelSeries.exe` and may not be up to date with the latest Sonar API. Use at your own risk.

> ⚠️ **Warning:** This project is unofficial and not affiliated with SteelSeries.

## Features

-   Typed client (`SonarClient`) with async methods for Sonar endpoints
-   DTO contracts generated from the OpenAPI schema
-   Uses `System.Text.Json` for serialization

## Requirements

-   .NET Standard 2.1 compatible runtime
-   SteelSeries Sonar running locally with its API base URL available

## Installation

Install the NuGet package:

```
dotnet add <your-project> package Wedol.SteelSeries.Sonar
```

## Usage

```csharp
using System.Net.Http;
using Wedol.SteelSeries.Sonar;

var baseUrl = "<sonar-api-base-url>";
using var httpClient = new HttpClient();
var client = new SonarClient(baseUrl, httpClient);

var routes = await client.AudioDeviceRoutingAllAsync();
```

## Regenerating the client

The client is generated from `swagger.json` using `nswag.json` and the `NSwag.MSBuild` package. Update the OpenAPI document and rebuild (Debug configuration) to regenerate `SonarClient.cs` and `Contracts.cs`.