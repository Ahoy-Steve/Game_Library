using System.Collections;
using SharpQuest.Contracts;

namespace Capstone.Core.Storage;

/// <summary>
/// The store. A list keeps insertion order; a dictionary makes lookups fast.
/// Both have to agree, so every change goes through Add and Remove.
/// </summary>
public sealed class GameLibrary : IItemStore, IEnumerable<IItem>
{
    //IItem change to Game
    private readonly List<IItem> _ordered = [];
    private readonly Dictionary<string, IItem> _byId = new(StringComparer.Ordinal);

    public int Count => _ordered.Count;

    // ---- Lab 02: the store ------------------------------------------------------------------

    public void Add(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!_byId.TryAdd(item.Id, item))
            throw new ArgumentException($"A game with Id '{item.Id}' is already in the library.", nameof(item));

        _ordered.Add(item);
    }

    public IItem? FindById(string id) => id is null ? null : _byId.GetValueOrDefault(id);

    // ToArray copies, so callers get a snapshot rather than a live view of _ordered.
    public IReadOnlyList<IItem> All() => _ordered.ToArray();

    public bool Remove(string id)
    {
        if (id is null || !_byId.Remove(id, out var item))
            return false;

        _ordered.Remove(item);
        return true;
    }

    // ---- Lab 05: the collection -------------------------------------------------------------

    /// <summary>The game at this position, counting from the first one added.</summary>
    public IItem this[int index]
    {
        get
        {
            if (index < 0 || index >= _ordered.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"The library holds {_ordered.Count} games; there is no position {index}.");

            return _ordered[index];
        }
    }

    /// <summary>The game with this Id. No such Id is an answer, not a bug: it returns null.</summary>
    public IItem? this[string id] => id is null ? null : _byId.GetValueOrDefault(id);
    
    public IEnumerator<IItem> GetEnumerator()
    {
        foreach (var item in _ordered)
            yield return item;
    }

    // The old, untyped one. foreach never calls it, but the interface demands it.
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public IEnumerable<IItem> Matching(string text)
    {
        
        ArgumentNullException.ThrowIfNull(text);

        return MatchingIterator(text);
    }

    private IEnumerable<IItem> MatchingIterator(string text)
    {
        foreach (var item in _ordered)
        {
            if (item.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase))
                yield return item;
        }
    }
}
