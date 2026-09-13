using DSAExperimentation.LeetCode.DesignAStackWithIncrementOperation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAStackWithIncrementOperation;

// Harness only: both strategies live in DesignAStackWithIncrementOperationSolution.
// LeetCode's own shape here is a stateful object across a sequence of calls, so
// each example is a maxSize plus a call script instead of a single argument tuple -
// the same shape MaximumFrequencyStackTests and MinStackTests already use for their
// own instance-API problems. The pre-migration test only exercised the two-Stack
// drain-and-rebuild composition; CreateByIndexedList (previously an untested,
// stack-less increment loop inlined in DesignAStackWithIncrementOperationBenchmarks
// as its [Benchmark(Baseline = true)] arm) gets the identical assertions here for
// the first time.
public sealed class DesignAStackWithIncrementOperationTests
{
    public static TheoryData<int, CustomStackOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example: maxSize 3, the sixth push dropped
                // because the stack is already full, then two overlapping
                // increments before the stack is drained.
                3,
                [
                    CustomStackOp.Push(1),
                    CustomStackOp.Push(2),
                    CustomStackOp.Pop(),
                    CustomStackOp.Push(2),
                    CustomStackOp.Push(3),
                    CustomStackOp.Push(4),
                    CustomStackOp.Increment(5, 100),
                    CustomStackOp.Increment(2, 100),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                ],
                [null, null, 2, null, null, null, null, null, 103, 202, 201, -1]
            },
            {
                // The pre-migration test's own sequence: one increment whose k
                // exceeds the size, so every element shifts, and a final pop off an
                // empty stack reporting -1 rather than throwing.
                3,
                [
                    CustomStackOp.Push(1),
                    CustomStackOp.Push(2),
                    CustomStackOp.Pop(),
                    CustomStackOp.Push(2),
                    CustomStackOp.Push(3),
                    CustomStackOp.Push(4),
                    CustomStackOp.Increment(5, 100),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                ],
                [null, null, 2, null, null, null, null, 103, 102, 101, -1]
            },
            {
                // Fewer elements than k: every element is incremented.
                5,
                [
                    CustomStackOp.Push(10),
                    CustomStackOp.Push(20),
                    CustomStackOp.Increment(10, 5),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                ],
                [null, null, null, 25, 15]
            },
            {
                // Increment touches only the bottom k, leaving the top untouched.
                4,
                [
                    CustomStackOp.Push(1),
                    CustomStackOp.Push(2),
                    CustomStackOp.Push(3),
                    CustomStackOp.Increment(2, 10),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                ],
                [null, null, null, null, 3, 12, 11]
            },
            {
                // Increment on an empty stack, and k = 0 on a non-empty one: both
                // no-ops.
                2,
                [
                    CustomStackOp.Increment(3, 7),
                    CustomStackOp.Pop(),
                    CustomStackOp.Push(8),
                    CustomStackOp.Increment(0, 7),
                    CustomStackOp.Pop(),
                ],
                [null, -1, null, null, 8]
            },
            {
                // maxSize 1: the second push is dropped, so the first value is what
                // both the increment and the pop see.
                1,
                [
                    CustomStackOp.Push(6),
                    CustomStackOp.Push(9),
                    CustomStackOp.Increment(1, 4),
                    CustomStackOp.Pop(),
                    CustomStackOp.Pop(),
                ],
                [null, null, null, 10, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByStackDrain_LeetCodeExamples_CapsPushesAndIncrementsBottomK(
        int maxSize, CustomStackOp[] operations, int?[] expected) =>
        RunScript(DesignAStackWithIncrementOperationSolution.CreateByStackDrain(maxSize), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIndexedList_LeetCodeExamples_CapsPushesAndIncrementsBottomK(
        int maxSize, CustomStackOp[] operations, int?[] expected) =>
        RunScript(DesignAStackWithIncrementOperationSolution.CreateByIndexedList(maxSize), operations, expected);

    private static void RunScript(
        DesignAStackWithIncrementOperationSolution.ICustomStack stack,
        CustomStackOp[] operations,
        int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(stack));
        }
    }
}

// One call in a CustomStack script: push a value, pop, or increment the bottom k
// elements. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays. Push and
// Increment return null (no return value); Pop returns the popped value - the same
// null-means-"no return value" convention FreqStackOp.Apply uses.
public readonly record struct CustomStackOp
{
    private readonly OpKind _kind;
    private readonly int _first;
    private readonly int _second;

    private CustomStackOp(OpKind kind, int first, int second)
    {
        _kind = kind;
        _first = first;
        _second = second;
    }

    private enum OpKind
    {
        Push,
        Pop,
        Increment,
    }

    public static CustomStackOp Push(int value) => new(OpKind.Push, value, 0);

    public static CustomStackOp Pop() => new(OpKind.Pop, 0, 0);

    public static CustomStackOp Increment(int k, int val) => new(OpKind.Increment, k, val);

    internal int? Apply(DesignAStackWithIncrementOperationSolution.ICustomStack stack)
    {
        switch (_kind)
        {
            case OpKind.Pop:
                return stack.Pop();
            case OpKind.Increment:
                stack.Increment(_first, _second);
                return null;
            default:
                stack.Push(_first);
                return null;
        }
    }
}
