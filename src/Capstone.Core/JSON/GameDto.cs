using Capstone.Core.Model;

namespace Capstone.Core.Json;

public sealed record GameDto(
    string? Id,
    string? Title,
    string? DeveloperName,
    Platform? Platform,
    string? Genre,
    int? ReleaseYear,
    Status? Status,
    double? HoursPlayed,
    decimal? PricePaid,
    int? Rating,
    string? Shelf = null,
    string? Store = null,
    string? Service = null,
    DateOnly? LeavesOn = null);