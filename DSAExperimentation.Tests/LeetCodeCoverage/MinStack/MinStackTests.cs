using DSAExperimentation.LeetCode.MinStack;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinStack;

// Harness only: the algorithm lives in MinStackSolution. LeetCode's own shape
// here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape LRUCacheTests
// already uses for its own instance-API problem. Both of MinStackSolution's arms
// are replayed here, through the IMinStackOperations both of them implement.
public sealed partial class MinStackTests
{
    public static TheoryData<MinStackOp[], int?[]> Examples =>
        new()
        {
            {
                [
                    MinStackOp.Push(-2),
                    MinStackOp.Push(0),
                    MinStackOp.Push(-3),
                    MinStackOp.GetMin(),
                    MinStackOp.Pop(),
                    MinStackOp.Top(),
                    MinStackOp.GetMin(),
                ],
                [null, null, null, -3, null, 0, -2]
            },

            // Popping the minimum restores the one beneath it.
            {
                [
                    MinStackOp.Push(5),
                    MinStackOp.Push(1),
                    MinStackOp.GetMin(),
                    MinStackOp.Pop(),
                    MinStackOp.GetMin(),
                ],
                [null, null, 1, null, 5]
            },
            {
                [
                    MinStackOp.Push(5),
                    MinStackOp.Push(5),
                    MinStackOp.GetMin(),
                    MinStackOp.Pop(),
                    MinStackOp.GetMin(),
                    MinStackOp.Push(3),
                    MinStackOp.GetMin(),
                    MinStackOp.Pop(),
                    MinStackOp.GetMin(),
                ],
                [null, null, 5, null, 5, null, 3, null, 5]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByStackPrimitive_LeetCodeExamples_TracksRunningMinimum(MinStackOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(MinStackSolution.CreateByStackPrimitive(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySingleListScan_LeetCodeExamples_TracksRunningMinimum(MinStackOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(MinStackSolution.CreateBySingleListScan(), operations));

    // The two arms are competing strategies for one question, so they must answer the same script
    // identically - including the tied-minimum example, where a value equal to the current minimum
    // is pushed and then popped over a deeper one.
    [Theory]
    [MemberData(nameof(Examples))]
    public void MinStack_ArmsAgreeOnEveryExample(MinStackOp[] operations, int?[] expected) =>
        Assert.Equal(
            RunScript(MinStackSolution.CreateByStackPrimitive(), operations),
            RunScript(MinStackSolution.CreateBySingleListScan(), operations));

    private static int?[] RunScript(MinStackSolution.IMinStackOperations stack, MinStackOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(stack))];

    // One call in a MinStack script: which operation to invoke and with what
    // argument. Pure dispatch, built via the named factories below so a script
    // (like Examples above) reads like the LeetCode call sequence it replays.
    // Nested here rather than left at file scope so the file declares exactly
    // one type.
    public readonly record struct MinStackOp(MinStackOp.OpKind kind, int value)
    {
        public static MinStackOp Push(int value) => new(OpKind.Push, value);

        public static MinStackOp Pop() => new(OpKind.Pop, 0);

        public static MinStackOp Top() => new(OpKind.Top, 0);

        public static MinStackOp GetMin() => new(OpKind.GetMin, 0);

        // null for push/pop, the returned value for top/getMin - so a script
        // runner can assert against one expected value per operation uniformly.
        // Internal, not public: only this same assembly's test method ever calls
        // Apply.
        internal int? Apply(MinStackSolution.IMinStackOperations stack)
        {
            switch (kind)
            {
                case OpKind.Push:
                    stack.Push(value);
                    return null;
                case OpKind.Pop:
                    stack.Pop();
                    return null;
                case OpKind.Top:
                    return stack.Top();
                default:
                    return stack.GetMin();
            }
        }

        public enum OpKind
        {
            Push,
            Pop,
            Top,
            GetMin,
        }
    }
}
