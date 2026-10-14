using Capstone.Core.Model;

namespace Capstone.Core.Json;

/// <summary>
/// The shape of a game on disk. Everything is nullable on purpose: JSON from outside
/// can be missing anything, and the mapper decides what is acceptable.
/// </summary>
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
    int? Rating);
