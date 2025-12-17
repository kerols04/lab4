using System.Text.Json.Serialization;

namespace ApiProject
{
    // Modell som matchar de fält vi vill läsa från GitHub JSON
    public sealed class GithubRepo
    {
        // JSON "name" -> C# Name (stor bokstav i C#)
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        // Beskrivning kan vara null i vissa repos
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        // Länken till repot på GitHub
        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        // Homepage kan vara tom/null
        [JsonPropertyName("homepage")]
        public string? Homepage { get; set; }

        // GitHub brukar ge watchers_count som “rätta” värdet
        [JsonPropertyName("watchers_count")]
        public int WatchersCount { get; set; }

        // Vissa svar kan även innehålla "watchers" (fallback)
        [JsonPropertyName("watchers")]
        public int WatchersLegacy { get; set; }

        // Vi väljer bästa värdet automatiskt
        [JsonIgnore]
        public int Watchers => WatchersCount != 0 ? WatchersCount : WatchersLegacy;

        // Senast någon pushade till repot
        [JsonPropertyName("pushed_at")]
        public DateTimeOffset? PushedAt { get; set; }
    }
}

