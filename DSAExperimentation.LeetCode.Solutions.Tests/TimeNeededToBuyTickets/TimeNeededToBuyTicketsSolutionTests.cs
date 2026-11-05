using DSAExperimentation.LeetCode.Solutions.TimeNeededToBuyTickets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TimeNeededToBuyTickets;

// Harness only. Both strategies are TimeNeededToBuyTicketsSolution's - this file
// pins them to the cases that separate the
// closed form from the simulation: targetPerson at the front of the line (everyone
// behind them loses a turn), targetPerson wanting a single ticket (everyone behind
// them contributes nothing at all), and a one-person line.
public sealed partial class TimeNeededToBuyTicketsSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [2, 3, 2], 0, 4 },
            { [3, 1, 2], 1, 2 },
            { [1, 1, 1], 2, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TimeRequiredToBuyByQueueSimulation_Cases_ReturnsSecondPersonKFinishes(
        int[] tickets, int targetPerson, int expected)
    {
        var actual = TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByQueueSimulation(tickets, targetPerson);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void TimeRequiredToBuyByClosedFormSum_Cases_ReturnsSecondPersonKFinishes(
        int[] tickets, int targetPerson, int expected)
    {
        var actual = TimeNeededToBuyTicketsSolution.TimeRequiredToBuyByClosedFormSum(tickets, targetPerson);

        Assert.Equal(expected, actual);
    }
}
