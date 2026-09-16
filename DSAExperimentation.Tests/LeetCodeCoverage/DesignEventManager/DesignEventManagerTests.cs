using DSAExperimentation.LeetCode.DesignEventManager;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignEventManager;

// Harness only. Both strategies are DesignEventManagerSolution's - this file
// replays LeetCode's published call sequence against each IEventManagerStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of mutating
// calls rather than a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem.
// EventManagerOp.Apply is pure dispatch (which method to call with which
// arguments) - no priority/ordering logic of its own.
public sealed class DesignEventManagerTests
{
    public static TheoryData<(int EventId, int Priority)[], EventManagerOp[], int?[]> Examples =>
        new()
        {
            {
                [(5, 7), (2, 7), (9, 4)],
                [
                    EventManagerOp.PollHighest(),
                    EventManagerOp.UpdatePriority(9, 7),
                    EventManagerOp.PollHighest(),
                    EventManagerOp.PollHighest(),
                ],
                [2, null, 5, 9]
            },
            {
                [(4, 1), (7, 2)],
                [
                    EventManagerOp.PollHighest(),
                    EventManagerOp.PollHighest(),
                    EventManagerOp.PollHighest(),
                ],
                [7, 4, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EventManagerByLinearScan_LeetCodeExamples_PollsHighestPriorityEventFirst(
        (int EventId, int Priority)[] initialEvents, EventManagerOp[] operations, int?[] expected) =>
        RunScript(new DesignEventManagerSolution.EventManagerByLinearScan(initialEvents), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void EventManagerByLazyDeletionHeap_LeetCodeExamples_PollsHighestPriorityEventFirst(
        (int EventId, int Priority)[] initialEvents, EventManagerOp[] operations, int?[] expected) =>
        RunScript(new DesignEventManagerSolution.EventManagerByLazyDeletionHeap(initialEvents), operations, expected);

    private static void RunScript(
        DesignEventManagerSolution.IEventManagerStrategy strategy,
        EventManagerOp[] operations,
        int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}
