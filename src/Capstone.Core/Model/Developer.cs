namespace Capstone.Core.Model;

/// <summary>
/// The studio behind a game. It's plain data (equal values mean the same studio), so a record.
/// In session 11 this becomes the parent of a one-to-many relationship; for now a Game just
/// carries its developer's name.
/// </summary>
public sealed record Developer(string Name, string Country, int Founded) : IDescribable
{
    public string Describe() => $"{Name} ({Country}, founded {Founded})";

    /// <summary>The studios behind the games in data/games.json.</summary>
    public static IReadOnlyList<Developer> Known { get; } =
    [
        new("CD Projekt Red", "Poland", 1994),
        new("FromSoftware", "Japan", 1986),
        new("Nintendo", "Japan", 1889),
        new("Valve", "United States", 1996),
        new("Naughty Dog", "United States", 1984),
        new("Larian Studios", "Belgium", 1996),
        new("Supergiant Games", "United States", 2009),
        new("Rockstar Games", "United States", 1998),
        new("Team Cherry", "Australia", 2014),
        new("Bethesda Game Studios", "United States", 2001),
    ];
}
