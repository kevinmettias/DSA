using static DSAExperimentation.LeetCode.DesignCircularQueue.DesignCircularQueueSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularQueue;

// Harness only: the algorithm lives in DesignCircularQueueSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem.
// CircularQueueOp.Apply encodes LeetCode's own bool results as 1/0 alongside
// Front/Rear's int results, matching the mixed bool/int shape LeetCode's own judge
// output already uses for this call sequence.
public sealed class DesignCircularQueueTests
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
        RunScript(new CircularQueueByArrayBacked(capacity), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CircularQueueByDequeBacked_LeetCodeExamples_MatchesExpectedSequence(
        int capacity, CircularQueueOp[] operations, int[] expected) =>
        RunScript(new CircularQueueByDequeBacked(capacity), operations, expected);

    private static void RunScript(ICircularQueue queue, CircularQueueOp[] operations, int[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(queue));
        }
    }
}

// One call in a CircularQueue script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays. Internal, not
// public: only this same assembly's test method ever calls Apply.
public readonly record struct CircularQueueOp
{
    private readonly Kind _kind;
    private readonly int _value;

    private CircularQueueOp(Kind kind, int value)
    {
        _kind = kind;
        _value = value;
    }

    public static CircularQueueOp EnQueue(int value) => new(Kind.EnQueue, value);

    public static CircularQueueOp DeQueue() => new(Kind.DeQueue, 0);

    public static CircularQueueOp Front() => new(Kind.Front, 0);

    public static CircularQueueOp Rear() => new(Kind.Rear, 0);

    public static CircularQueueOp IsEmpty() => new(Kind.IsEmpty, 0);

    public static CircularQueueOp IsFull() => new(Kind.IsFull, 0);

    // 1/0 for the four bool-returning operations, the actual value for
    // Front/Rear - so a script runner can assert against one expected value per
    // operation uniformly.
    internal int Apply(ICircularQueue queue) => _kind switch
    {
        Kind.EnQueue => queue.EnQueue(_value) ? 1 : 0,
        Kind.DeQueue => queue.DeQueue() ? 1 : 0,
        Kind.Front => queue.Front(),
        Kind.Rear => queue.Rear(),
        Kind.IsEmpty => queue.IsEmpty() ? 1 : 0,
        _ => queue.IsFull() ? 1 : 0,
    };

    private enum Kind
    {
        EnQueue,
        DeQueue,
        Front,
        Rear,
        IsEmpty,
        IsFull,
    }
}
