namespace Capstone.Core.Model;

/// <summary>A game you bought and downloaded, from a store such as Steam or GOG.</summary>
public sealed class DigitalGame : Game
{
    private readonly string _store = "";

    public required string Store
    {
        get => _store;
        init => _store = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A digital game needs a store.", nameof(Store))
            : value.Trim();
    }

    public override string Describe() => $"{Card()} · digital, bought on {Store}";
}
