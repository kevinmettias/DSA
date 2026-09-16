using DSAExperimentation.LeetCode.TimeNeededToBuyTickets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TimeNeededToBuyTickets;

// Harness only. Both strategies are TimeNeededToBuyTicketsSolution's - this file
// pins them to LeetCode's published examples plus the cases that separate the
// closed form from the simulation: k at the front of the line (everyone behind
// them loses a turn), k wanting a single ticket (everyone behind them contributes
// nothing at all), and a one-person line.
public sealed class TimeNeededToBuyTicketsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [2, 3, 2], 2, 6 },
            { [5, 1, 1, 1], 0, 8 },
            { [2, 3, 2], 0, 4 },
            { [3, 1, 2], 1, 2 },
            { [1, 1, 1], 2, 3 },
            { [1], 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TimeRequiredToBuyByQueueSimulation_LeetCodeExamples_ReturnsSecondPersonKFinishes(
        int[] tickets, int k, int expected)
    {
        var actual = TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByQueueSimulation(tickets, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TimeRequiredToBuyByClosedFormSum_LeetCodeExamples_ReturnsSecondPersonKFinishes(
        int[] tickets, int k, int expected)
    {
        var actual = TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByClosedFormSum(tickets, k);

        Assert.Equal(expected, actual);
    }
}
