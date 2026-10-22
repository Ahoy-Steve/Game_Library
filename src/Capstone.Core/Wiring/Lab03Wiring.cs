using Capstone.Core.Model;
using Capstone.Core.Storage;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab03Wiring : ILab03Wiring
{
    private static readonly (string Title, string Developer, string Genre)[] Samples =
    [
        ("Hollow Knight", "Team Cherry", "Metroidvania"),
        ("Elden Ring", "FromSoftware", "Action RPG"),
        ("Hades", "Supergiant Games", "Roguelike"),
        ("Baldur's Gate 3", "Larian Studios", "RPG"),
        ("Portal 2", "Valve", "Puzzle"),
    ];

    private static readonly Platform[] Platforms = Enum.GetValues<Platform>();
    private static readonly Status[] Statuses = Enum.GetValues<Status>();

    public IItemStore CreateStore() => new GameLibrary();

    /// <summary>A valid Game. Same seed, same Id; different seeds, different Ids.</summary>
    public IItem CreateSample(int seed)
    {
        var index = Math.Abs(seed);
        var (title, developer, genre) = Samples[index % Samples.Length];

        return new Game
        {
            Id = $"gm-{seed:D3}",
            Title = title,
            DeveloperName = developer,
            Platform = Platforms[index % Platforms.Length],
            Genre = genre,
            ReleaseYear = 2000 + index % 25,
            Status = Statuses[index % Statuses.Length],
            HoursPlayed = index % 40,
            PricePaid = 10m + index % 50,
            Rating = index % 3 == 0 ? null : 1 + index % 10,
        };
    }
    
    public IReadOnlyList<IItem> Search(IItemStore store, string query)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(query);

        var matcher = SearchMatcher.Create();
        return store.All().Where(item => matcher.Matches(item.DisplayName, query)).ToList();
    }
}