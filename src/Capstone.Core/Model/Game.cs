using SharpQuest.Contracts;

namespace Capstone.Core.Model;

public sealed class Game : IItem, IDescribable
{
    public const int FirstReleaseYear = 1950;
    public const int LastReleaseYear = 2100;
    public const int MinRating = 1;
    public const int MaxRating = 10;
    private readonly string _id = "";
    private readonly string _title = "";
    private readonly string _developerName = "";
    private readonly string _genre = "";
    private readonly int _releaseYear = FirstReleaseYear;
    private double _hoursPlayed;
    private decimal _pricePaid;
    private int? _rating;

    public required string Id
    {
        get => _id;
        init => _id = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A game needs an Id.", nameof(Id))
            : value;
    }

    public required string Title
    {
        get => _title;
        init => _title = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A game needs a title.", nameof(Title))
            : value.Trim();
    }

    public required string DeveloperName
    {
        get => _developerName;
        init => _developerName = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A game needs a developer.", nameof(DeveloperName))
            : value.Trim();
    }

    public required Platform Platform { get; init; }

    public required string Genre
    {
        get => _genre;
        init => _genre = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A game needs a genre.", nameof(Genre))
            : value.Trim();
    }

    public required int ReleaseYear
    {
        get => _releaseYear;
        init => _releaseYear = value is >= FirstReleaseYear and <= LastReleaseYear
            ? value
            : throw new ArgumentOutOfRangeException(nameof(ReleaseYear), value,
                $"Release year is between {FirstReleaseYear} and {LastReleaseYear}.");
    }

    public required Status Status { get; set; }

    /// <summary>Hours you actually played. Yours, not the API's. Zero is a real answer: bought, never started.</summary>
    public double HoursPlayed
    {
        get => _hoursPlayed;
        set => _hoursPlayed = value is >= 0 and < double.PositiveInfinity
            ? value
            : throw new ArgumentOutOfRangeException(nameof(HoursPlayed), value, "Hours played can't be negative.");
    }

    /// <summary>What you paid. Zero is real too: a gift, a giveaway, a subscription.</summary>
    public decimal PricePaid
    {
        get => _pricePaid;
        set => _pricePaid = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(PricePaid), value, "Price paid can't be negative.");
    }

    /// <summary>1 to 10. Null means "haven't rated it", which is not the same as a zero.</summary>
    public int? Rating
    {
        get => _rating;
        set => _rating = value is null or (>= MinRating and <= MaxRating)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Rating), value,
                $"Rating is between {MinRating} and {MaxRating}, or null for unrated.");
    }

    public string DisplayName => Title;

    public string Describe()
    {
        var rating = Rating is { } r ? $"{r}/10" : "unrated";
        return $"{Title} · {Platform} · {Genre} · {Status} · {HoursPlayed:0.#} h · {rating}";
    }

    public override string ToString() => Describe();
}
