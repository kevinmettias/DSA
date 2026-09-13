using DSAExperimentation.LeetCode.MaximumFrequencyStack;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumFrequencyStack;

// Harness only: both strategies live in MaximumFrequencyStackSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes a
// call script instead of a single argument tuple - the same shape MinStackTests and
// AllOneDataStructureTests already use for their own instance-API problems. The
// pre-migration test only exercised the HashMap+Stack composition; CreateByListRescan
// (previously untested scaffolding inlined in MaximumFrequencyStackBenchmarks as its
// [Benchmark(Baseline = true)] arm) gets the identical assertions here for the first
// time.
public sealed class MaximumFrequencyStackTests
{
    public static TheoryData<FreqStackOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example: push 5,7,5,7,4,5 then four pops.
                [
                    FreqStackOp.Push(5),
                    FreqStackOp.Push(7),
                    FreqStackOp.Push(5),
                    FreqStackOp.Push(7),
                    FreqStackOp.Push(4),
                    FreqStackOp.Push(5),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                ],
                [null, null, null, null, null, null, 5, 7, 5, 4]
            },
            {
                [FreqStackOp.Push(42), FreqStackOp.Pop()],
                [null, 42]
            },
            {
                // Pushes interleaved with pops, so a frequency that was already
                // vacated has to be reachable again: after the first pop the stack
                // is 1,2 at frequency 1 apiece, and pushing 2 recreates frequency 2.
                [
                    FreqStackOp.Push(1),
                    FreqStackOp.Push(1),
                    FreqStackOp.Push(2),
                    FreqStackOp.Pop(),
                    FreqStackOp.Push(2),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                ],
                [null, null, null, 1, null, 2, 2, 1]
            },
            {
                // One value only: every pop returns it, and the maximum frequency
                // walks all the way back down to zero.
                [
                    FreqStackOp.Push(9),
                    FreqStackOp.Push(9),
                    FreqStackOp.Push(9),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                ],
                [null, null, null, 9, 9, 9]
            },
            {
                // All distinct, so every value sits at frequency 1 and Pop degrades
                // to plain LIFO order.
                [
                    FreqStackOp.Push(1),
                    FreqStackOp.Push(2),
                    FreqStackOp.Push(3),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                    FreqStackOp.Pop(),
                ],
                [null, null, null, 3, 2, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashMapAndStack_LeetCodeExamples_PopsMostFrequentThenMostRecent(
        FreqStackOp[] operations, int?[] expected) =>
        RunScript(MaximumFrequencyStackSolution.CreateByHashMapAndStack(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByListRescan_LeetCodeExamples_PopsMostFrequentThenMostRecent(
        FreqStackOp[] operations, int?[] expected) =>
        RunScript(MaximumFrequencyStackSolution.CreateByListRescan(), operations, expected);

    private static void RunScript(
        MaximumFrequencyStackSolution.IFreqStack freqStack, FreqStackOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(freqStack));
        }
    }
}

// One call in a FreqStack script: push a value, or pop. Pure dispatch, built via the
// named factories below so a script (like Examples above) reads like the LeetCode call
// sequence it replays. Push returns null (no return value); Pop returns the popped
// value - the same null-means-"no return value" convention MinStackOp.Apply uses.
public readonly record struct FreqStackOp
{
    private readonly bool _isPush;
    private readonly int _value;

    private FreqStackOp(bool isPush, int value)
    {
        _isPush = isPush;
        _value = value;
    }

    public static FreqStackOp Push(int value) => new(true, value);

    public static FreqStackOp Pop() => new(false, 0);

    internal int? Apply(MaximumFrequencyStackSolution.IFreqStack freqStack)
    {
        if (!_isPush)
        {
            return freqStack.Pop();
        }

        freqStack.Push(_value);
        return null;
    }
}
