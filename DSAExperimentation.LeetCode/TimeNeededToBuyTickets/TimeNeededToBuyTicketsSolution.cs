using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Index, int Remaining)>;

namespace DSAExperimentation.LeetCode.TimeNeededToBuyTickets;

// LeetCode 2073. Time Needed to Buy Tickets: everyone in a line buys one ticket
// per second and immediately rejoins the back of the line if they still want more.
// Report the second at which person k buys their last ticket and leaves.
//
// The two strategies attack it from opposite ends. One replays the line move by
// move; the other observes that person i only ever gets min(tickets[i], tickets[k])
// turns - one fewer when they stand behind k, because k's final purchase stops the
// clock before their next turn comes round.
internal static class TimeNeededToBuyTicketsSolution
{
    // Person k's own turn is what ends the day, so nobody standing behind them
    // reaches their last possible turn.
    private const int TurnsLostBehindK = 1;

    // This repo's own Queue<(int, int)> simulating the line directly: every person
    // is Enqueued once with their remaining ticket count, and each turn TryDequeue's
    // whoever is at the front, sells them one ticket, and re-Enqueues them at the
    // back if they still want more - the "rotate to the back, re-enqueue
    // immediately" idiom FindTheWinnerOfTheCircularGame already establishes for this
    // Queue<T>. Costs one turn of work per ticket actually sold.
    public static int TimeRequiredToBuyByQueueSimulation(int[] tickets, int k)
    {
        var line = new RepoQueue();

        for (var index = 0; index < tickets.Length; index++)
        {
            line.Enqueue((index, tickets[index]));
        }

        var time = 0;

        while (line.TryDequeue(out var person))
        {
            time++;

            if (!ServeOneTicket(person, k, line))
            {
                break;
            }
        }

        return time;
    }

    // Sells one ticket to `person`; re-enqueues them at the back if they still want
    // more. Returns false once person k has bought their last ticket, the signal to
    // stop the simulation.
    private static bool ServeOneTicket((int Index, int Remaining) person, int k, RepoQueue line)
    {
        var remaining = person.Remaining - 1;

        if (remaining == 0 && person.Index == k)
        {
            return false;
        }

        if (remaining > 0)
        {
            line.Enqueue((person.Index, remaining));
        }

        return true;
    }

    // The O(n) closed form, reading the answer off the ticket counts without ever
    // modelling the line: person k buys tickets[k] tickets, so everyone at or ahead
    // of k contributes min(tickets[i], tickets[k]) seconds and everyone behind them
    // one fewer. Plain arithmetic over the input array - the arm the simulation has
    // to justify itself against.
    public static int TimeRequiredToBuyByClosedFormSum(int[] tickets, int k)
    {
        var target = tickets[k];
        var time = 0;

        for (var index = 0; index < tickets.Length; index++)
        {
            var turns = index <= k ? target : TurnsBehindK(target);
            time += Math.Min(tickets[index], turns);
        }

        return time;
    }

    // Everyone behind person k loses that last turn, once k has bought out.
    private static int TurnsBehindK(int target) => target - TurnsLostBehindK;
}
