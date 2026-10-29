namespace Capstone.Core.Collections;

/// <summary>
/// The last N distinct values, newest first. One class, one letter, any T:
/// recently played games, recent searches, recent years.
/// </summary>
public sealed class RecentlyUsed<T>
{
    private readonly int _capacity;
    private readonly List<T> _items = [];   // index 0 is the newest

    public RecentlyUsed(int capacity)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "A capacity below 1 can't hold anything.");

        _capacity = capacity;
    }

    public int Capacity => _capacity;

    /// <summary>What it holds right now, newest first. A snapshot: changing it doesn't change the bag.</summary>
    public IReadOnlyList<T> Items => _items.ToArray();

    /// <summary>Put a value in front. If it is already in there it moves, it doesn't take a second place.</summary>
    public void Push(T value)
    {
        // EqualityComparer<T>.Default, not ==: this is what makes it work for any T.
        var comparer = EqualityComparer<T>.Default;

        var existing = _items.FindIndex(x => comparer.Equals(x, value));
        if (existing >= 0)
            _items.RemoveAt(existing);

        _items.Insert(0, value);

        if (_items.Count > _capacity)
            _items.RemoveAt(_items.Count - 1);   // the oldest drops out
    }
}
