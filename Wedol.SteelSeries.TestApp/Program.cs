using System.Text.Json;
using Wedol.SteelSeries.Sonar;

namespace Wedol.SteelSeries.TestApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        SteelSeriesGG ssgg = new SteelSeriesGG();

        SonarClient sonarClient = await ssgg.GetSonarClientAsync();

        var volumes = await sonarClient.GetVolumesettingsClassicAsync();
        Console.WriteLine(JsonSerializer.Serialize(volumes, jsonOptions));
    }
}
