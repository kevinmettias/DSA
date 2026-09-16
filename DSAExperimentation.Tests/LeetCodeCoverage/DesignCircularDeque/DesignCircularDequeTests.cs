using DSAExperimentation.LeetCode.DesignCircularDeque;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularDeque;

// Harness only: the algorithm lives in DesignCircularDequeSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem.
// CircularDequeOp.Apply encodes LeetCode's own bool results as 1/0 alongside
// GetFront/GetRear's int results, matching the mixed bool/int shape LeetCode's own
// judge output already uses for this call sequence.
public sealed partial class DesignCircularDequeTests
{
    public static TheoryData<int, CircularDequeOp[], int[]> Examples =>
        new()
        {
            {
                3,
                [
                    CircularDequeOp.InsertLast(1),
                    CircularDequeOp.InsertLast(2),
                    CircularDequeOp.InsertFront(3),
                    CircularDequeOp.InsertFront(4),
                    CircularDequeOp.GetRear(),
                    CircularDequeOp.IsFull(),
                    CircularDequeOp.DeleteLast(),
                    CircularDequeOp.InsertFront(4),
                    CircularDequeOp.GetFront(),
                ],
                [1, 1, 1, 0, 2, 1, 1, 1, 4]
            },
            {
                1,
                [
                    CircularDequeOp.IsEmpty(),
                    CircularDequeOp.DeleteFront(),
                    CircularDequeOp.DeleteLast(),
                    CircularDequeOp.GetFront(),
                    CircularDequeOp.GetRear(),
                ],
                [1, 0, 0, -1, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularDequeByArrayBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularDequeOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignCircularDequeSolution.CircularDequeByArrayBacked(capacity), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularDequeByDequeBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularDequeOp[] operations, int[] expected) =>
        Assert.Equal(expected, RunScript(new DesignCircularDequeSolution.CircularDequeByDequeBacked(capacity), operations));

    private static int[] RunScript(
        DesignCircularDequeSolution.ICircularDeque deque,
        CircularDequeOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(deque))];
}
