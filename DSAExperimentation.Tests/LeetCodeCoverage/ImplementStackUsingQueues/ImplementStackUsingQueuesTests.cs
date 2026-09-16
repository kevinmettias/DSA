using DSAExperimentation.LeetCode.ImplementStackUsingQueues;

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
        RunScript(ImplementStackUsingQueuesSolution.CreateByBuiltInQueue(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByQueuePrimitive_LeetCodeExamples_BehavesLifo(StackOp[] operations, object?[] expected) =>
        RunScript(ImplementStackUsingQueuesSolution.CreateByQueuePrimitive(), operations, expected);

    private static void RunScript(
        ImplementStackUsingQueuesSolution.IStackOperations stack, StackOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(stack));
        }
    }
}
