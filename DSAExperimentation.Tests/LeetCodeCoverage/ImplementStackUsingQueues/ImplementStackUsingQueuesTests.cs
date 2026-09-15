using static DSAExperimentation.LeetCode.ImplementStackUsingQueues.ImplementStackUsingQueuesSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementStackUsingQueues;

// Harness only. Both strategies are ImplementStackUsingQueuesSolution's - this
// file replays LeetCode's published call sequence against each
// IStackOperations implementation via a small operation script, so a failure
// still names the strategy that broke even though the "input" here is a
// sequence of mutating calls rather than a single argument tuple. StackOp.Apply
// is pure dispatch (which method to call with which argument) - no rotation
// logic of its own.
public sealed class ImplementStackUsingQueuesTests
{
    public static TheoryData<StackOp[], object?[]> Examples =>
        new()
        {
            // LeetCode's own example: push(1), push(2), top(), pop(), empty().
            {
                [StackOp.Push(1), StackOp.Push(2), StackOp.Top(), StackOp.Pop(), StackOp.Empty()],
                [null, null, 2, 2, false]
            },
            // A deeper LIFO check: push three, pop/top down to empty.
            {
                [
                    StackOp.Push(5),
                    StackOp.Push(13),
                    StackOp.Push(2),
                    StackOp.Top(),
                    StackOp.Pop(),
                    StackOp.Top(),
                    StackOp.Pop(),
                    StackOp.Empty(),
                    StackOp.Pop(),
                    StackOp.Empty(),
                ],
                [null, null, null, 2, 2, 13, 13, false, 5, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBuiltInQueue_LeetCodeExamples_BehavesLifo(StackOp[] operations, object?[] expected) =>
        RunScript(CreateByBuiltInQueue(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByQueuePrimitive_LeetCodeExamples_BehavesLifo(StackOp[] operations, object?[] expected) =>
        RunScript(CreateByQueuePrimitive(), operations, expected);

    private static void RunScript(IStackOperations stack, StackOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(stack));
        }
    }
}

// One call in a MyStack script: which operation to invoke and with what
// argument. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct StackOp(StackOp.OpKind kind, int value)
{
    public static StackOp Push(int value) => new(OpKind.Push, value);

    public static StackOp Pop() => new(OpKind.Pop, 0);

    public static StackOp Top() => new(OpKind.Top, 0);

    public static StackOp Empty() => new(OpKind.Empty, 0);

    // null for push, the int result for pop/top, the bool result for empty -
    // so a script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IStackOperations is internal to
    // ImplementStackUsingQueuesSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal object? Apply(IStackOperations stack)
    {
        switch (kind)
        {
            case OpKind.Push:
                stack.Push(value);
                return null;
            case OpKind.Pop:
                return stack.Pop();
            case OpKind.Top:
                return stack.Top();
            default:
                return stack.Empty();
        }
    }

    public enum OpKind
    {
        Push,
        Pop,
        Top,
        Empty,
    }
}
