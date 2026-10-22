namespace Capstone.Core.Model;

public static class GameClassifier
{
    public static string Classify(SharpQuest.Contracts.IItem item) => item switch
    {
        null => throw new ArgumentNullException(nameof(item)),
        PhysicalGame => "on the shelf",
        DigitalGame => "in the launcher",
        SubscriptionGame => "rented",
        _ => "something else",
    };
    
    
    public static string PlayAdvice(Game game, DateOnly today) => game switch
    {
        null => throw new ArgumentNullException(nameof(game)),
        { Status: Status.Completed or Status.Abandoned } => "done with it",
        SubscriptionGame { LeavesOn: { } day } when day.DayNumber - today.DayNumber <= 30 => "play it now",
        SubscriptionGame => "no rush, it's on the service",
        { Status: Status.Playing } => "keep going",
        _ => "whenever",
    };
}
