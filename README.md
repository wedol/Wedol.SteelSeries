# Wedol.SteelSeries

A .NET Standard 2.1 client for the SteelSeries Sonar local API, generated from `swagger.json` using NSwag.

Tested with SteelSeries GG software version 10.4.1.0. The `swagger.json` file was extracted from `SteelSeriesSonar.exe` and may not be up to date with the latest Sonar API. Use at your own risk.

> ⚠️ **Warning:** This project is unofficial and not affiliated with SteelSeries.

## Features

-   Typed client (`SonarClient`) with async methods for Sonar endpoints
-   Automatic initialization through `SteelSeriesGG` class that loads configuration from SteelSeries GG program data
-   DTO contracts generated from the OpenAPI schema
-   Uses `System.Text.Json` for serialization
-   Support for custom `HttpClient` instances

## Requirements

-   .NET Standard 2.1 compatible runtime
-   SteelSeries GG installed with its program data accessible
-   SteelSeries Sonar running and enabled

## Installation

Install the NuGet package:

```
dotnet add <your-project> package Wedol.SteelSeries
```

## Usage

### Using SteelSeriesGG Initialization (Recommended)

The `SteelSeriesGG` class automatically loads configuration from the SteelSeries GG program data directory and provides easy access to the Sonar client:

```csharp
using Wedol.SteelSeries;
using Wedol.SteelSeries.Sonar;

var steelSeriesGG = new SteelSeriesGG();
var sonarClient = await steelSeriesGG.GetSonarClientAsync();

var volumes = await sonarClient.GetVolumesettingsClassicAsync();
```

#### Custom HttpClient

You can provide your own `HttpClient` instance:

```csharp
using var httpClient = new HttpClient();
var sonarClient = await steelSeriesGG.GetSonarClientAsync(httpClient);
```

#### Custom Program Data Path

If SteelSeries GG is installed in a non-standard location, you can specify the program data path:

```csharp
var steelSeriesGG = new SteelSeriesGG("/path/to/SteelSeries/GG");
var sonarClient = await steelSeriesGG.GetSonarClientAsync();
```

### Direct Sonar Client Usage

Alternatively, you can directly instantiate the `SonarClient` if you already know the Sonar API base URL:

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