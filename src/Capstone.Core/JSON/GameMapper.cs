using System.Text.Json;
using System.Text.Json.Serialization;
using Capstone.Core.Model;
using SharpQuest.Contracts;

namespace Capstone.Core.Json;

public sealed class GameMapper : IItemMapper
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        // Enums travel as names ("Backlog"), and a bare number like 99 is not accepted as a Status.
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public string ToJson(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item is not Game g)
            throw new ArgumentException($"This mapper only knows games, not {item.GetType().Name}.", nameof(item));

        return JsonSerializer.Serialize(ToDto(g), Options);
    }

    public IItem? FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        GameDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<GameDto>(json, Options);
        }
        catch (JsonException)
        {
            return null; // not JSON, or not an object, or a value of the wrong type
        }

        return dto is null ? null : FromDto(dto);
    }

    public static GameDto ToDto(Game g)
    {
        var dto = new GameDto(g.Id, g.Title, g.DeveloperName, g.Platform, g.Genre, g.ReleaseYear,
            g.Status, g.HoursPlayed, g.PricePaid, g.Rating);

        // Each kind writes down the one thing that is its own.
        return g switch
        {
            PhysicalGame p => dto with { Shelf = p.Shelf },
            SubscriptionGame s => dto with { Service = s.Service, LeavesOn = s.LeavesOn },
            DigitalGame d => dto with { Store = d.Store },
            _ => dto,
        };
    }

    /// <summary>Null when the DTO is missing something a game cannot exist without, or holds a value out of range.</summary>
    public static Game? FromDto(GameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id)) return null;
        if (string.IsNullOrWhiteSpace(dto.Title)) return null;
        if (string.IsNullOrWhiteSpace(dto.DeveloperName)) return null;
        if (string.IsNullOrWhiteSpace(dto.Genre)) return null;
        if (dto.Platform is not { } platform || !Enum.IsDefined(platform)) return null;
        if (dto.Status is not { } status || !Enum.IsDefined(status)) return null;
        if (dto.ReleaseYear is not { } year || year is < Game.FirstReleaseYear or > Game.LastReleaseYear) return null;

        // Hours and price are optional: a game you just bought has played 0 hours. Negative is never valid.
        var hours = dto.HoursPlayed ?? 0;
        if (hours < 0 || double.IsNaN(hours) || double.IsInfinity(hours)) return null;
        var price = dto.PricePaid ?? 0m;
        if (price < 0) return null;

        // Null rating means "not rated"; anything else must be on the scale.
        if (dto.Rating is < Game.MinRating or > Game.MaxRating) return null;

        //  Wherever we used to write "new Game", we now pick one.
        // A blank shelf or service is not "absent", it's invalid, so those return null too.
        if (dto.Shelf is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Shelf)) return null;
            return new PhysicalGame
            {
                Id = dto.Id,
                Title = dto.Title,
                DeveloperName = dto.DeveloperName,
                Platform = platform,
                Genre = dto.Genre,
                ReleaseYear = year,
                Status = status,
                HoursPlayed = hours,
                PricePaid = price,
                Rating = dto.Rating,
                Shelf = dto.Shelf,
            };
        }

        if (dto.Service is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Service)) return null;
            return new SubscriptionGame
            {
                Id = dto.Id,
                Title = dto.Title,
                DeveloperName = dto.DeveloperName,
                Platform = platform,
                Genre = dto.Genre,
                ReleaseYear = year,
                Status = status,
                HoursPlayed = hours,
                PricePaid = price,
                Rating = dto.Rating,
                Service = dto.Service,
                LeavesOn = dto.LeavesOn,
            };
        }

        // No shelf, no service: a plain download. (Older files have neither, so they load as digital.)
        if (dto.Store is not null && string.IsNullOrWhiteSpace(dto.Store)) return null;
        return new DigitalGame
        {
            Id = dto.Id,
            Title = dto.Title,
            DeveloperName = dto.DeveloperName,
            Platform = platform,
            Genre = dto.Genre,
            ReleaseYear = year,
            Status = status,
            HoursPlayed = hours,
            PricePaid = price,
            Rating = dto.Rating,
            Store = dto.Store ?? "Unknown",
        };
    }
}
