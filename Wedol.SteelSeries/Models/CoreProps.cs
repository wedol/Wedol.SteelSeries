
using System.Text.Json.Serialization;

namespace Wedol.SteelSeries.Models
{
    internal class CoreProps
    {
        [JsonPropertyName("encryptedAddress")]
        public string? EncryptedAddress { get; set; }

        [JsonPropertyName("ggEncryptedAddress")]
        public string? GGEncryptedAddress { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }
    }
}
