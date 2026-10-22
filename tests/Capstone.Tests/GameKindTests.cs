using Capstone.Core.Json;
using Capstone.Core.Model;

namespace Capstone.Tests;

// Session 4: three kinds of game, one base. These check what is different about each kind.
public class GameKindTests
{
    private static PhysicalGame Physical() => new()
    {
        Id = "gm-p", Title = "Hades", DeveloperName = "Supergiant Games", Platform = Platform.Switch,
        Genre = "Roguelike", ReleaseYear = 2020, Status = Status.Backlog, Shelf = "A2",
    };

    private static SubscriptionGame Subscription(DateOnly? leavesOn, Status status = Status.Backlog) => new()
    {
        Id = "gm-s", Title = "Hi-Fi Rush", DeveloperName = "Tango Gameworks", Platform = Platform.Xbox,
        Genre = "Rhythm", ReleaseYear = 2023, Status = status, Service = "Game Pass", LeavesOn = leavesOn,
    };

    [Fact]
    public void A_physical_game_says_where_it_is_on_the_shelf()
    {
        Assert.Contains("shelf A2", Physical().Describe());
    }

    [Fact]
    public void A_subscription_game_says_when_it_leaves()
    {
        Assert.Contains("2027-03-01", Subscription(new DateOnly(2027, 3, 1)).Describe());
    }

    [Fact]
    public void A_physical_game_with_a_blank_shelf_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new PhysicalGame
        {
            Id = "gm-x", Title = "Hades", DeveloperName = "Supergiant Games", Platform = Platform.PC,
            Genre = "Roguelike", ReleaseYear = 2020, Status = Status.Backlog, Shelf = "  ",
        });
    }

    [Theory]
    [InlineData(10, "play it now")]
    [InlineData(30, "play it now")]
    [InlineData(31, "no rush, it's on the service")]
    public void A_subscription_game_leaving_within_30_days_is_play_it_now(int daysLeft, string expected)
    {
        var today = new DateOnly(2026, 10, 22);

        Assert.Equal(expected, GameClassifier.PlayAdvice(Subscription(today.AddDays(daysLeft)), today));
    }

    [Fact]
    public void A_finished_subscription_game_is_not_play_it_now()
    {
        var today = new DateOnly(2026, 10, 22);

        Assert.Equal("done with it", GameClassifier.PlayAdvice(Subscription(today.AddDays(5), Status.Completed), today));
    }

    [Fact]
    public void Every_kind_comes_back_from_json_as_its_own_kind()
    {
        var mapper = new GameMapper();

        foreach (Game original in new Game[] { Physical(), Subscription(new DateOnly(2027, 3, 1)) })
        {
            var copy = mapper.FromJson(mapper.ToJson(original));

            Assert.IsType(original.GetType(), copy);
        }
    }

    [Fact]
    public void Json_without_a_shelf_or_service_loads_as_a_digital_game()
    {
        var game = new GameMapper().FromJson(
            """{"id":"gm-1","title":"Portal 2","developerName":"Valve","platform":"PC","genre":"Puzzle","releaseYear":2011,"status":"Completed"}""");

        Assert.IsType<DigitalGame>(game);
    }
}
