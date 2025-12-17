using System.Net.Http.Headers;
using System.Text.Json;

namespace ApiProject
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // En HttpClient räcker (återanvänds för båda API:erna)
            using var http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20) // undviker att appen hänger
            };

            // GitHub kräver att vi skickar User-Agent, annars kan vi få 403
            http.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("ApiProject", "1.0"));

            // Bra standard för JSON-responser från GitHub
            http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            // Options: gör deserialisering tolerant mot casing i JSON
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            await ShowGitHubReposAsync(http, jsonOptions);
            Console.WriteLine();
            await ShowMontvaleAsync(http, jsonOptions);

            Console.WriteLine("\nTryck valfri tangent för att avsluta...");
            Console.ReadKey();
        }

        private static string Clean(string? s)
            => string.IsNullOrWhiteSpace(s) ? "-" : s.Trim();

        private static string FormatDate(DateTimeOffset? dt)
            => dt is null ? "-" : dt.Value.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        private static async Task ShowGitHubReposAsync(HttpClient http, JsonSerializerOptions jsonOptions)
        {
            const string url = "https://api.github.com/orgs/dotnet/repos";

            Console.WriteLine("=== GitHub API: .NET Foundation repos ===\n");

            try
            {
                // 1) Hämta JSON som text
                var json = await http.GetStringAsync(url);

                // 2) Deserialisera JSON -> List<GithubRepo>
                var repos = JsonSerializer.Deserialize<List<GithubRepo>>(json, jsonOptions) ?? new();

                // Visar ett begränsat antal, annars blir output enorm
                const int maxToShow = 5;
                Console.WriteLine($"Hittade {repos.Count} repos. Visar {Math.Min(maxToShow, repos.Count)} st:\n");

                foreach (var r in repos.Take(maxToShow))
                {
                    Console.WriteLine($"Name:        {Clean(r.Name)}");
                    Console.WriteLine($"Homepage:    {Clean(r.Homepage)}");
                    Console.WriteLine($"GitHub:      {Clean(r.HtmlUrl)}");
                    Console.WriteLine($"Description: {Clean(r.Description)}");
                    Console.WriteLine($"Watchers:    {r.Watchers}");
                    Console.WriteLine($"Last push:   {FormatDate(r.PushedAt)}");
                    Console.WriteLine();
                }
            }
            catch (HttpRequestException ex)
            {
                // Typiskt fel: rate limit, nätproblem, 403 osv
                Console.WriteLine("HTTP-fel mot GitHub:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Tips: Om du får rate limit, vänta och kör igen.");
            }
            catch (JsonException ex)
            {
                // JSON-format oväntat (ovanligt men bra att fånga)
                Console.WriteLine("JSON-fel: kunde inte tolka GitHub-svaret.");
                Console.WriteLine(ex.Message);
            }
            catch (TaskCanceledException)
            {
                // Timeout hamnar ofta här
                Console.WriteLine("Timeout: GitHub-svaret tog för lång tid.");
            }
        }

        private static async Task ShowMontvaleAsync(HttpClient http, JsonSerializerOptions jsonOptions)
        {
            // VG: Montvale, New Jersey via Zippopotam.us
            const string url = "https://api.zippopotam.us/us/nj/montvale";

            Console.WriteLine("=== VG: Zippopotam.us – Montvale, New Jersey ===\n");

            try
            {
                // 1) Hämta JSON
                var json = await http.GetStringAsync(url);

                // 2) Deserialisera
                var data = JsonSerializer.Deserialize<ZippopotamResponse>(json, jsonOptions);

                if (data is null || data.Places.Count == 0)
                {
                    Console.WriteLine("Kunde inte hitta platsinfo för Montvale.");
                    return;
                }

                // Tar första träffen (räcker för uppgiften)
                var place = data.Places[0];

                // Post code kan finnas på root eller på place (vi tar det som finns)
                var postCode = Clean(place.PostCode) != "-" ? place.PostCode : data.PostCode;

                Console.WriteLine($"Place:      {Clean(place.PlaceName)}, {Clean(data.State)}");
                Console.WriteLine($"Postnummer: {Clean(postCode)}");
                Console.WriteLine($"Latitud:    {Clean(place.Latitude)}");
                Console.WriteLine($"Longitud:   {Clean(place.Longitude)}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP-fel mot Zippopotam:");
                Console.WriteLine(ex.Message);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON-fel: kunde inte tolka Zippopotam-svaret.");
                Console.WriteLine(ex.Message);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Timeout: Zippopotam-svaret tog för lång tid.");
            }
        }
    }
}

