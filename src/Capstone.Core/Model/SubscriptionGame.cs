namespace Capstone.Core.Model;

/// <summary>A game you can play only while a service (Game Pass, PS Plus...) keeps it.</summary>
public sealed class SubscriptionGame : Game
{
    private readonly string _service = "";

    public required string Service
    {
        get => _service;
        init => _service = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("A subscription game needs a service.", nameof(Service))
            : value.Trim();
    }

    /// <summary>The day it leaves the service. Null means nobody has announced one.</summary>
    public DateOnly? LeavesOn { get; init; }

    public override string Describe() =>
        LeavesOn is { } day
            ? $"{Card()} · on {Service}, leaves {day:yyyy-MM-dd}"
            : $"{Card()} · on {Service}, no leaving date";
}
