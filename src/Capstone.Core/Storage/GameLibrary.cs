using SharpQuest.Contracts;

namespace Capstone.Core.Storage;

/// <summary>
/// The store. A list keeps insertion order; a dictionary makes lookups fast.
/// Both have to agree, so every change goes through Add and Remove.
/// </summary>
public sealed class GameLibrary : IItemStore
{
    private readonly List<IItem> _ordered = [];
    private readonly Dictionary<string, IItem> _byId = new(StringComparer.Ordinal);

    public int Count => _ordered.Count;

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
}
