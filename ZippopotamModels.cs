using System.Text.Json.Serialization;

namespace ApiProject
{
    // Modell för Zippopotam.us svar
    public sealed class ZippopotamResponse
    {
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        // I vissa endpoints kan post code ligga på root-nivå
        [JsonPropertyName("post code")]
        public string? PostCode { get; set; }

        // Själva platserna (kan vara flera)
        [JsonPropertyName("places")]
        public List<ZippopotamPlace> Places { get; set; } = new();
    }

    public sealed class ZippopotamPlace
    {
        [JsonPropertyName("place name")]
        public string? PlaceName { get; set; }

        // Vissa endpoints kan även ge post code här
        [JsonPropertyName("post code")]
        public string? PostCode { get; set; }

        [JsonPropertyName("latitude")]
        public string? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; }
    }
}

