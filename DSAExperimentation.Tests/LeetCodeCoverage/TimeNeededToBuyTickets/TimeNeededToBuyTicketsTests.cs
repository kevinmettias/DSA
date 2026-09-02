using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Index, int Remaining)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TimeNeededToBuyTickets;

// LeetCode 2073. Time Needed to Buy Tickets: this repo's own Queue<(int,int)>
// simulates the line directly - every person is Enqueued once with their
// remaining ticket count, and each turn TryDequeue's the person at the front, buys
// one ticket, and (if they still want more) re-Enqueues them at the back, exactly
// the "rotate to the back, re-enqueue immediately" idiom
// FindTheWinnerOfTheCircularGameTests already establishes for this Queue<T>.
public sealed partial class TimeNeededToBuyTicketsTests
{
    [Theory]
    [InlineData(new[] { 2, 3, 2 }, 2, 6)]
    [InlineData(new[] { 5, 1, 1, 1 }, 0, 8)]
    public void TimeRequiredToBuy_LeetCodeExamples_ReturnsExpectedTime(int[] tickets, int k, int expected)
    {
        var actual = TimeRequiredToBuy(tickets, k);
        Assert.Equal(expected, actual);
    }

    private static int TimeRequiredToBuy(int[] tickets, int k)
    {
        var line = new RepoQueue();

        for (var i = 0; i < tickets.Length; i++)
        {
            line.Enqueue((i, tickets[i]));
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
}
