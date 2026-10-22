using Capstone.Core.Model;

namespace Capstone.Tests;

public class GameTests
{
    // A small helper that lives inside this class only: a valid Game, with the
    // one or two things a test cares about changed. No other test file is needed.
    private static Game NewGame(
        string id = "gm-001",
        string title = "Hades",
        Platform platform = Platform.PC,
        int releaseYear = 2020)
    {
        return new DigitalGame
        {
            Id = id,
            Title = title,
            DeveloperName = "Supergiant Games",
            Platform = platform,
            Genre = "Roguelike",
            ReleaseYear = releaseYear,
            Status = Status.Backlog,
            Store = "Steam",
        };
    }

    // --- Validation: Rating (1 to 10, or null for "unrated") ---

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void Rating_outside_1_to_10_is_rejected(int rating)
    {
        var game = NewGame();

        Assert.Throws<ArgumentOutOfRangeException>(() => game.Rating = rating);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Rating_inside_1_to_10_is_accepted(int rating)
    {
        var game = NewGame();

        game.Rating = rating;

        Assert.Equal(rating, game.Rating);
    }

    [Fact]
    public void Rating_can_be_null_meaning_unrated()
    {
        var game = NewGame();
        game.Rating = 7;

        game.Rating = null;

        Assert.Null(game.Rating);
    }

    [Fact]
    public void A_rejected_rating_leaves_the_old_rating_in_place()
    {
        var game = NewGame();
        game.Rating = 7;

        Assert.Throws<ArgumentOutOfRangeException>(() => game.Rating = 11);

        Assert.Equal(7, game.Rating);
    }

    // --- Validation: HoursPlayed and PricePaid (zero is fine, negative is not) ---

    [Fact]
    public void Negative_hours_played_is_rejected()
    {
        var game = NewGame();

        Assert.Throws<ArgumentOutOfRangeException>(() => game.HoursPlayed = -0.5);
    }

    [Fact]
    public void Zero_hours_played_is_allowed()
    {
        var game = NewGame();

        game.HoursPlayed = 0;

        Assert.Equal(0, game.HoursPlayed);
    }

    [Fact]
    public void Negative_price_paid_is_rejected()
    {
        var game = NewGame();

        Assert.Throws<ArgumentOutOfRangeException>(() => game.PricePaid = -1m);
    }

    [Fact]
    public void A_free_game_with_price_zero_is_allowed()
    {
        var game = NewGame();

        game.PricePaid = 0m;

        Assert.Equal(0m, game.PricePaid);
    }

    // --- Validation: required text fields and release year ---

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void A_blank_title_is_rejected(string title)
    {
        Assert.Throws<ArgumentException>(() => NewGame(title: title));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void A_blank_id_is_rejected(string id)
    {
        Assert.Throws<ArgumentException>(() => NewGame(id: id));
    }

    [Theory]
    [InlineData(1949)]
    [InlineData(2101)]
    public void Release_year_outside_the_allowed_range_is_rejected(int year)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NewGame(releaseYear: year));
    }

    // --- Calculated properties and text ---

    [Fact]
    public void Title_is_trimmed()
    {
        var game = NewGame(title: "  Hades  ");

        Assert.Equal("Hades", game.Title);
    }

    [Fact]
    public void DisplayName_is_the_title()
    {
        var game = NewGame(title: "Portal 2");

        Assert.Equal("Portal 2", game.DisplayName);
    }

    [Fact]
    public void Describe_says_unrated_when_there_is_no_rating()
    {
        var game = NewGame();

        Assert.Contains("unrated", game.Describe());
    }

    [Fact]
    public void Describe_shows_the_rating_out_of_10_when_there_is_one()
    {
        var game = NewGame();
        game.Rating = 8;

        Assert.Contains("8/10", game.Describe());
    }

    [Fact]
    public void Describe_includes_title_platform_and_hours()
    {
        var game = NewGame(title: "Hades", platform: Platform.Switch);
        game.HoursPlayed = 12;

        var text = game.Describe();

        Assert.Contains("Hades", text);
        Assert.Contains("Switch", text);
        Assert.Contains("12 h", text);
    }
}

    