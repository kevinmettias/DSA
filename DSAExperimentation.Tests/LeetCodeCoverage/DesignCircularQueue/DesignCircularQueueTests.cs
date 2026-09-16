using DSAExperimentation.LeetCode.DesignCircularQueue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularQueue;

// Harness only: the algorithm lives in DesignCircularQueueSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem.
// CircularQueueOp.Apply encodes LeetCode's own bool results as 1/0 alongside
// Front/Rear's int results, matching the mixed bool/int shape LeetCode's own judge
// output already uses for this call sequence.
public sealed partial class DesignCircularQueueTests
{
    public static TheoryData<int, CircularQueueOp[], int[]> Examples =>
        new()
        {
            {
                3,
                [
                    CircularQueueOp.EnQueue(1),
                    CircularQueueOp.EnQueue(2),
                    CircularQueueOp.EnQueue(3),
                    CircularQueueOp.EnQueue(4),
                    CircularQueueOp.Rear(),
                    CircularQueueOp.IsFull(),
                    CircularQueueOp.DeQueue(),
                    CircularQueueOp.EnQueue(4),
                    CircularQueueOp.Rear(),
                    CircularQueueOp.Front(),
                ],
                [1, 1, 1, 0, 3, 1, 1, 1, 4, 2]
            },
            {
                1,
                [
                    CircularQueueOp.IsEmpty(),
                    CircularQueueOp.DeQueue(),
                    CircularQueueOp.IsEmpty(),
                ],
                [1, 0, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularQueueByArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularQueueOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignCircularQueueSolution.CircularQueueByArrayBacked(capacity), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularQueueByDequeBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularQueueOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignCircularQueueSolution.CircularQueueByDequeBacked(capacity), operations));

    private static int[] RunScript(
        DesignCircularQueueSolution.ICircularQueue queue,
        CircularQueueOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(queue))];
}
