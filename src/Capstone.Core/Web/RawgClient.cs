using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Capstone.Core.Model;

namespace Capstone.Core.Web;
 /// <summary>
/// Reads one game's facts from https://rawg.io. Two things are passed in, never created or hard-coded here:
/// the HttpClient (one for the whole program, not one per call) and the API key
/// (read from the RAWG_API_KEY environment variable by the caller, never written in source).
/// RAWG knows genres and release dates. It does not know how long YOU played, so hours stay at 0.
/// </summary>
public sealed class RawgClient(HttpClient http, string apiKey)
{
    /// <summary>The game, or null if RAWG has never heard of it. Other failures throw.</summary>
    public async Task<Game?> GetAsync(string slugOrId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slugOrId);

        var url = $"games/{Uri.EscapeDataString(slugOrId.Trim().ToLowerInvariant())}?key={Uri.EscapeDataString(apiKey)}";
        using var response = await http.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode(); // 401 (bad key), 429 (too many requests)... are exceptions, not nulls

        var body = await response.Content.ReadFromJsonAsync<ApiGame>(cancellationToken);
        if (body is null || string.IsNullOrWhiteSpace(body.Name))
            return null;

        var year = DateTime.TryParse(body.Released, CultureInfo.InvariantCulture, DateTimeStyles.None, out var released)
            ? released.Year
            : (int?)null;
        if (year is null) return null; // a game without a release date can't become one of ours

        // RAWG doesn't know how YOU own it, so this starts as a download from an unknown store.
        return new DigitalGame
        {
            Id = $"rawg-{body.Id}",
            Title = body.Name,
            DeveloperName = body.Developers?.FirstOrDefault()?.Name ?? "Unknown",
            Platform = body.Platforms?.Select(p => ParsePlatform(p.Platform?.Name)).FirstOrDefault(p => p is not null) ?? Platform.PC,
            Genre = body.Genres?.FirstOrDefault()?.Name ?? "Unknown",
            ReleaseYear = year.Value,
            Status = Status.Backlog,
            Store = "Unknown",
        };
    }

    private static Platform? ParsePlatform(string? name) => name switch
    {
        null => null,
        _ when name.StartsWith("PlayStation", StringComparison.OrdinalIgnoreCase) => Platform.PlayStation,
        _ when name.StartsWith("Xbox", StringComparison.OrdinalIgnoreCase) => Platform.Xbox,
        _ when name.StartsWith("Nintendo Switch", StringComparison.OrdinalIgnoreCase) => Platform.Switch,
        _ when name.Equals("PC", StringComparison.OrdinalIgnoreCase) => Platform.PC,
        _ when name.Equals("Android", StringComparison.OrdinalIgnoreCase) || name.Equals("iOS", StringComparison.OrdinalIgnoreCase) => Platform.Mobile,
        _ => null,
    };

    // Only the parts of RAWG's response we use. Everything else is ignored by the deserializer.
    private sealed record ApiGame(
        int Id,
        string? Name,
        string? Released,
        List<ApiNamed>? Genres,
        List<ApiNamed>? Developers,
        List<ApiPlatformSlot>? Platforms);

    private sealed record ApiNamed(string? Name);

    private sealed record ApiPlatformSlot([property: JsonPropertyName("platform")] ApiNamed? Platform);
}
