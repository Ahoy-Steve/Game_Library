using Capstone.Core.Functions;
using Capstone.Core.Storage;
using SharpQuest.Contracts;

namespace Capstone.Core.Wiring;

public sealed class Lab06Wiring : ILab06Wiring
{
    public IItemStore CreateStore() => new GameLibrary();
    
    public IItem CreateSample(int seed) => new Lab02Wiring().CreateSample(seed);
    
    public Type PublisherType => typeof(GameLibrary);

    public Action Subscribe(IItemStore store, Action<IItem> handler)
    {
        var mine = store as GameLibrary
                   ?? throw new ArgumentException($"Expected a {nameof(GameLibrary)}.", nameof(store));
        ArgumentNullException.ThrowIfNull(handler);

        mine.ItemAdded += handler;
        return () => mine.ItemAdded -= handler;
    }

    public IReadOnlyList<IItem> Where(IItemStore store, Func<IItem, bool> keep) => ItemFunctions.Where(store, keep);

    public IReadOnlyList<string> Map(IItemStore store, Func<IItem, string> describe) => ItemFunctions.Map(store, describe);

    public void ForEach(IItemStore store, Action<IItem> action) => ItemFunctions.ForEach(store, action);
}