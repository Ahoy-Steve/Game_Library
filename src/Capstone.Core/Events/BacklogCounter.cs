using Capstone.Core.Model;
using SharpQuest.Contracts;

namespace Capstone.Core.Events;

/// <summary>
/// Subscriber one: keeps a running tally of games added to the backlog.
/// The library knows nothing about it. It only hands over "a game was added".
/// </summary>
public sealed class BacklogCounter
{
    public int Games { get; private set; }
    
    public decimal MoneyTiedUp { get; private set; }

    public void Handle(IItem item)
    {
        if (item is Game { Status: Status.Backlog } game)
        {
            Games++;
            MoneyTiedUp += game.PricePaid;
        }
    }
}