using static DSAExperimentation.LeetCode.DesignCircularDeque.DesignCircularDequeSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularDeque;

// Harness only: the algorithm lives in DesignCircularDequeSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignCircularQueueTests already uses for its own instance-API problem.
// CircularDequeOp.Apply encodes LeetCode's own bool results as 1/0 alongside
// GetFront/GetRear's int results, matching the mixed bool/int shape LeetCode's own
// judge output already uses for this call sequence.
public sealed class DesignCircularDequeTests
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
        RunScript(new CircularDequeByArrayBacked(capacity), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularDequeByDequeBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularDequeOp[] operations, int[] expected) =>
        RunScript(new CircularDequeByDequeBacked(capacity), operations, expected);

    private static void RunScript(ICircularDeque deque, CircularDequeOp[] operations, int[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(deque));
        }
    }
}

// One call in a CircularDeque script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays. Internal, not
// public: only this same assembly's test method ever calls Apply.
public readonly record struct CircularDequeOp(CircularDequeOp.OpKind kind, int value)
{
    public static CircularDequeOp InsertFront(int value) => new(OpKind.InsertFront, value);

    public static CircularDequeOp InsertLast(int value) => new(OpKind.InsertLast, value);

    public static CircularDequeOp DeleteFront() => new(OpKind.DeleteFront, 0);

    public static CircularDequeOp DeleteLast() => new(OpKind.DeleteLast, 0);

    public static CircularDequeOp GetFront() => new(OpKind.GetFront, 0);

    public static CircularDequeOp GetRear() => new(OpKind.GetRear, 0);

    public static CircularDequeOp IsEmpty() => new(OpKind.IsEmpty, 0);

    public static CircularDequeOp IsFull() => new(OpKind.IsFull, 0);

    // 1/0 for the four bool-returning operations, the actual value for
    // GetFront/GetRear - so a script runner can assert against one expected value
    // per operation uniformly.
    internal int Apply(ICircularDeque deque) => kind switch
    {
        OpKind.InsertFront => deque.InsertFront(value) ? 1 : 0,
        OpKind.InsertLast => deque.InsertLast(value) ? 1 : 0,
        OpKind.DeleteFront => deque.DeleteFront() ? 1 : 0,
        OpKind.DeleteLast => deque.DeleteLast() ? 1 : 0,
        OpKind.GetFront => deque.GetFront(),
        OpKind.GetRear => deque.GetRear(),
        OpKind.IsEmpty => deque.IsEmpty() ? 1 : 0,
        _ => deque.IsFull() ? 1 : 0,
    };

    public enum OpKind
    {
        InsertFront,
        InsertLast,
        DeleteFront,
        DeleteLast,
        GetFront,
        GetRear,
        IsEmpty,
        IsFull,
    }
}
