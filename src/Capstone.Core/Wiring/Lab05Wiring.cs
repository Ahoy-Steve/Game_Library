using Capstone.Core.Collections;
using Capstone.Core.Storage;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab05Wiring : ILab05Wiring
{
    public IItemStore CreateStore() => new GameLibrary();

    // Same contract as Lab 02, so we ask the Lab 02 wiring rather than copy its sample table.
    public IItem CreateSample(int seed) => new Lab02Wiring().CreateSample(seed);

    public Type CollectionType => typeof(GameLibrary);

    // Open definition: empty angle brackets, not RecentlyUsed<string>.
    public Type GenericDefinition => typeof(RecentlyUsed<>);

    public IItem At(IItemStore store, int index) => Library(store)[index];

    public IItem? ById(IItemStore store, string id) => Library(store)[id];

    // foreach over the store itself, not over All().
    public IReadOnlyList<IItem> EachInOrder(IItemStore store)
    {
        var items = new List<IItem>();
        foreach (var item in Library(store))
            items.Add(item);
        return items;
    }

    // Hands back the lazy query as it is. No ToList() here: that would make it eager.
    public IEnumerable<IItem> Matching(IItemStore store, string text) => Library(store).Matching(text);

    public IReadOnlyList<T> Recent<T>(int capacity, params T[] pushed)
    {
        var recent = new RecentlyUsed<T>(capacity);   // throws for capacity < 1
        foreach (var value in pushed)
            recent.Push(value);
        return recent.Items;
    }

    private static GameLibrary Library(IItemStore store) =>
        store as GameLibrary
        ?? throw new ArgumentException($"Expected a {nameof(GameLibrary)}, got {store?.GetType().Name ?? "null"}.", nameof(store));
}
