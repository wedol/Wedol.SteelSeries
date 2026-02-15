using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wedol.SteelSeries.Models;
using Wedol.SteelSeries.Sonar;

namespace Wedol.SteelSeries
{
    /// <summary>
    /// Provides functionality to initialize and interact with the SteelSeries GG application, including access to
    /// sub-application clients such as Sonar.
    /// </summary>
    /// <remarks>Loads configuration from the SteelSeries GG program data path and manages communication
    /// with sub-applications. This class is not thread-safe; concurrent access should be externally synchronized if
    /// used from multiple threads.</remarks>
    public sealed class SteelSeriesGG
    {
        private bool _initialized = false;
        private CoreProps? _coreProps = null;
        private JsonDocument? _subApps;

        public string SteelSeriesGGProgramDataPath { get; private set; }

        public SteelSeriesGG()
        {
            SteelSeriesGGProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SteelSeries", "GG");
        }

        public SteelSeriesGG(string SteelSeriesGGProgramDataPath)
        {
            this.SteelSeriesGGProgramDataPath = SteelSeriesGGProgramDataPath;
        }

        private async Task InitializeAsync(HttpClient client)
        {
            if (_initialized)
                return;

            if (!Directory.Exists(SteelSeriesGGProgramDataPath))
                throw new DirectoryNotFoundException($"The directory \"{SteelSeriesGGProgramDataPath}\" does not exist.");

            string corePropsPath = Path.Combine(SteelSeriesGGProgramDataPath, "coreProps.json");
            if (!File.Exists(corePropsPath))
                throw new FileNotFoundException($"The file \"{corePropsPath}\" does not exist.");

            _coreProps = JsonSerializer.Deserialize<CoreProps>(File.ReadAllText(corePropsPath));

            if (_coreProps == null)
                throw new InvalidOperationException("Failed to deserialize coreProps.json. The resulting CoreProps object is null.");
            if (string.IsNullOrEmpty(_coreProps.GGEncryptedAddress))
                throw new InvalidOperationException("Failed to initialize SteelSeriesGG. GGEncryptedAddress in coreProps.json is null or empty.");

            await LoadSubAppsAsync(client, _coreProps.GGEncryptedAddress);

            _initialized = true;
        }

        private async Task LoadSubAppsAsync(HttpClient client, string address)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://{address}/subApps");
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                _subApps = JsonDocument.Parse(jsonResponse);


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load subApps from SteelSeries GG. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Asynchronously retrieves a configured instance of the SonarClient for communicating with the Sonar
        /// sub-application in SteelSeries GG.
        /// </summary>
        /// <remarks>This method requires that SteelSeries GG is properly initialized and that the Sonar
        /// sub-application is available and enabled. It handles the creation of the HttpClient if one is not provided
        /// and ensures that all necessary properties are set before returning the SonarClient instance.</remarks>
        /// <param name="httpClient">The HttpClient to use for making HTTP requests. If null, a new HttpClient with a custom handler that accepts
        /// all server certificates is created.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a SonarClient instance
        /// configured with the Sonar web server address.</returns>
        /// <exception cref="InvalidOperationException">Thrown if SteelSeries GG initialization fails, if the Sonar sub-application is not enabled or running, or if
        /// the Sonar web server address cannot be retrieved.</exception>

        public async Task<SonarClient> GetSonarClientAsync(HttpClient? httpClient = null)
        {
            if (httpClient == null)
            {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                httpClient = new HttpClient(handler);
            }

            await InitializeAsync(httpClient);

            if (string.IsNullOrEmpty(_coreProps!.GGEncryptedAddress))
                throw new InvalidOperationException("Failed to initialize SteelSeriesGG. GGEncryptedAddress is null or empty.");

            string sonarUrl = "";

            try
            {
                JsonElement sonarElement = _subApps!.RootElement.GetProperty("subApps").GetProperty("sonar");

                if (!sonarElement.GetProperty("isEnabled").GetBoolean())
                    throw new InvalidOperationException("Sonar is not enabled in SteelSeries GG.");

                if (!sonarElement.GetProperty("isRunning").GetBoolean())
                    throw new InvalidOperationException("Sonar is not running.");

                sonarUrl = sonarElement.GetProperty("metadata").GetProperty("webServerAddress").GetString() ?? "";

                if (string.IsNullOrEmpty(sonarUrl))
                    throw new InvalidOperationException("Failed to retrieve Sonar web server address from subApps. The address is null or empty.");

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to find Sonar subApp in the loaded subApps. See inner exception for details.", ex);
            }

            return new SonarClient(sonarUrl, httpClient);
        }
    }
}
