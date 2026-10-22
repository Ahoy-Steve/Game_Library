using Capstone.Core.Model;
using Capstone.Core.Storage;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab04Wiring : ILab04Wiring
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

    public Type BaseType => typeof(Game);

    public IReadOnlyList<Type> KindTypes { get; } = [typeof(PhysicalGame), typeof(DigitalGame), typeof(SubscriptionGame)];

    /// <summary>Kind 0 = physical, 1 = digital, 2 = subscription. The kind is part of the Id, so no two collide.</summary>
    public IItem CreateOfKind(int kindIndex, int seed)
    {
        var index = Math.Abs(seed);
        var (title, developer, genre) = Samples[index % Samples.Length];
        var platform = Platforms[index % Platforms.Length];
        var status = Statuses[index % Statuses.Length];
        var hours = index % 40;
        var price = 10m + index % 50;
        int? rating = index % 3 == 0 ? null : 1 + index % 10;

        // Same seed, same game. Only the way you own it changes from kind to kind.
        return kindIndex switch
        {
            0 => new PhysicalGame
            {
                Id = $"phys-{seed:D3}", Title = title, DeveloperName = developer, Platform = platform, Genre = genre,
                ReleaseYear = 2000 + index % 25, Status = status, HoursPlayed = hours, PricePaid = price,
                Rating = rating, Shelf = $"A{index % 5 + 1}",
            },
            1 => new DigitalGame
            {
                Id = $"dig-{seed:D3}", Title = title, DeveloperName = developer, Platform = platform, Genre = genre,
                ReleaseYear = 2000 + index % 25, Status = status, HoursPlayed = hours, PricePaid = price,
                Rating = rating, Store = "Steam",
            },
            2 => new SubscriptionGame
            {
                Id = $"sub-{seed:D3}", Title = title, DeveloperName = developer, Platform = platform, Genre = genre,
                ReleaseYear = 2000 + index % 25, Status = status, HoursPlayed = hours, PricePaid = 0m,
                Rating = rating, Service = "Game Pass", LeavesOn = new DateOnly(2027, 1, 1).AddDays(index),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(kindIndex), kindIndex, "Kinds are 0, 1 and 2."),
        };
    }

    // The override decides. We only ask the item to describe itself.
    public string Describe(IItem item) =>
        item is IDescribable describable ? describable.Describe() : item.DisplayName;

    // One loop. It never asks which kind it is holding.
    public IReadOnlyList<string> DescribeAll(IItemStore store)
    {
        var lines = new List<string>();
        foreach (var item in store.All())
            lines.Add(Describe(item));
        return lines;
    }

    // The outside world sorts the item: a switch expression, not an override.
    public string Classify(IItem item) => GameClassifier.Classify(item);

    public bool TryFindByName(IItemStore store, string name, out IItem? found)
    {
        found = null;
        if (string.IsNullOrWhiteSpace(name)) return false;

        foreach (var item in store.All())
        {
            if (string.Equals(item.DisplayName, name, StringComparison.Ordinal))
            {
                found = item;
                return true;
            }
        }

        return false;
    }

    // params: AddAll(store), AddAll(store, a), AddAll(store, a, b, c). The store's own rules still apply.
    public int AddAll(IItemStore store, params IItem[] items)
    {
        ArgumentNullException.ThrowIfNull(items);

        var added = 0;
        foreach (var item in items)
        {
            store.Add(item);
            added++;
        }

        return added;
    }
}
