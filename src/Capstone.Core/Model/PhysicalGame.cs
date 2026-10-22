namespace Capstone.Core.Model;

/// <summary>A game you own on a disc or cartridge. It sits somewhere on a shelf.</summary>
public sealed class PhysicalGame : Game
{
    private readonly string _shelf = "";

    public required string Shelf
    {
        get => _shelf;
        init => _shelf = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A physical game needs a shelf.", nameof(Shelf))
            : value.Trim();
    }

    public override string Describe() => $"{Card()} · physical, on shelf {Shelf}";
}
