using DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementQueueUsingStacks;

// Harness only: the algorithm lives in ImplementQueueUsingStacksSolution.
// LeetCode's own shape here is a stateful object across a sequence of
// push/pop/peek/empty calls, so Examples encodes a call script instead of a
// single argument tuple - the same shape MinStackTests/LRUCacheTests use for
// their own instance-API problems. Only one strategy exists (the original
// benchmark's two [Benchmark] arms were both `return 1` placeholders, not a
// second algorithm to reconcile - see ImplementQueueUsingStacksSolution's own
// doc comment), so there is only one [Theory] method.
public sealed class ImplementQueueUsingStacksTests
{
    public static TheoryData<QueueOp[], object?[]> Examples =>
        new()
        {
            {
                [
                    QueueOp.Push(1),
                    QueueOp.Push(2),
                    QueueOp.Peek(),
                    QueueOp.Pop(),
                    QueueOp.Empty(),
                ],
                [null, null, 1, 1, false]
            },
            {
                [
                    QueueOp.Push(1),
                    QueueOp.Push(2),
                    QueueOp.Push(3),
                    QueueOp.Pop(),
                    QueueOp.Pop(),
                    QueueOp.Push(4),
                    QueueOp.Peek(),
                    QueueOp.Pop(),
                    QueueOp.Empty(),
                ],
                [null, null, null, 1, 2, null, 3, 3, false]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTwoStacks_LeetCodeExamples_BehavesFifo(QueueOp[] operations, object?[] expected)
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoStacks();

        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(queue));
        }
    }
}

// One call in a MyQueue script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct QueueOp(QueueOp.OpKind kind, int value)
{
    public static QueueOp Push(int value) => new(OpKind.Push, value);

    public static QueueOp Pop() => new(OpKind.Pop, 0);

    public static QueueOp Peek() => new(OpKind.Peek, 0);

    public static QueueOp Empty() => new(OpKind.Empty, 0);

    // null for push, the returned value for pop/peek/empty (boxed as its own
    // type - int or bool - so the harness can assert without forcing every
    // operation onto one numeric shape). Internal, not public: only this
    // same assembly's test method ever calls Apply.
    internal object? Apply(ImplementQueueUsingStacksSolution.TwoStackQueue queue)
    {
        switch (kind)
        {
            case OpKind.Push:
                queue.Push(value);
                return null;
            case OpKind.Pop:
                return queue.Pop();
            case OpKind.Peek:
                return queue.Peek();
            default:
                return queue.Empty();
        }
    }

    public enum OpKind
    {
        Push,
        Pop,
        Peek,
        Empty,
    }
}
