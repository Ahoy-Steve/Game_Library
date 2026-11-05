using SharpQuest.Contracts;

namespace Capstone.Core.Functions;

/// <summary>
/// Functions that take a function. Our code is the loop; the caller brings the rule.
/// Each one visits every item exactly once, in store order, and never second-guesses the caller's function.
/// </summary>
public static class ItemFunctions
{
    /// <summary>The items <paramref name="keep"/> says yes to.</summary>
    public static IReadOnlyList<IItem> Where(IItemStore store, Func<IItem, bool> keep)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(keep);

        var kept = new List<IItem>();
        foreach (var item in store.All())
        {
            if (keep(item))  
                kept.Add(item);
        }

        return kept;
    }

    /// <summary>One string per item, produced by <paramref name="describe"/>.</summary>
    public static IReadOnlyList<string> Map(IItemStore store, Func<IItem, string> describe)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(describe);

        var described = new List<string>();
        foreach (var item in store.All())
            described.Add(describe(item));

        return described;
    }

    /// <summary>Runs <paramref name="action"/> on every item. Returns nothing: the action is the point.</summary>
    public static void ForEach(IItemStore store, Action<IItem> action)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in store.All())
            action(item);
    }
}