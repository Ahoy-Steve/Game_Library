using System.Text.Json;
using Capstone.Core.Json;
using Capstone.Core.Model;
using Capstone.Core.Storage;
using Capstone.Core.Web;

// Lab 02: load the game library from JSON, skip what's invalid, show what's left.
// dotnet run --project src/Capstone.App                         → local file only
// dotnet run --project src/Capstone.App -- --online hades       → also asks RAWG about one game
//   (needs the free key in the RAWG_API_KEY environment variable. Never put it in code or in the JSON.)

var library = new GameLibrary();
var path = Path.Combine(AppContext.BaseDirectory, "data", "games.json");

using (var doc = JsonDocument.Parse(File.ReadAllText(path)))
{
    var mapper = new GameMapper();
    foreach (var element in doc.RootElement.EnumerateArray())
    {
        if (mapper.FromJson(element.GetRawText()) is not Game game)
            Console.WriteLine($"skipped invalid entry: {element.GetRawText()}");
        else if (library.FindById(game.Id) is not null)
            Console.WriteLine($"skipped duplicate Id: {game.Id}");
        else
            library.Add(game);
    }
}

Console.WriteLine($"{library.Count} games loaded:");
foreach (var item in library.All())
    Console.WriteLine($"  {(item as IDescribable)?.Describe() ?? item.DisplayName}");

Console.WriteLine();
Console.WriteLine("By status:");
foreach (var status in Enum.GetValues<Status>())
    Console.WriteLine($"  {status,-10} {library.All().OfType<Game>().Count(g => g.Status == status)}");

Console.WriteLine();
Console.WriteLine("Developers:");
foreach (IDescribable developer in Developer.Known)
    Console.WriteLine($"  {developer.Describe()}");

var onlineAt = Array.IndexOf(args, "--online");
if (onlineAt >= 0)
{
    var slug = onlineAt + 1 < args.Length ? args[onlineAt + 1] : "hades";
    var key = Environment.GetEnvironmentVariable("RAWG_API_KEY");
    if (string.IsNullOrWhiteSpace(key))
    {
        Console.WriteLine("Set the RAWG_API_KEY environment variable first (free key at https://rawg.io/apidocs).");
        return;
    }

    // One HttpClient for the program's lifetime. BaseAddress ends with '/', relative paths don't start with one.
    using var http = new HttpClient { BaseAddress = new Uri("https://api.rawg.io/api/"), Timeout = TimeSpan.FromSeconds(10) };
    var api = new RawgClient(http, key);
    try
    {
        var game = await api.GetAsync(slug);
        Console.WriteLine(game is null ? "RAWG doesn't know that one." : $"From RAWG: {game.Describe()}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"RAWG unreachable or refused the request: {ex.StatusCode?.ToString() ?? "no response"}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("RAWG took too long.");
    }
}
