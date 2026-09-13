using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignAStackWithIncrementOperation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignAStackWithIncrementOperationSolution's, the
// same factories DesignAStackWithIncrementOperationTests proves correct. A BCL
// List<int> whose indexer reaches the bottom min(k, size) slots directly (O(k) per
// Increment) vs. this repo's own Stack<int>, whose public surface is deliberately
// LIFO-only (Push/TryPop/TryPeek, no indexer - see Stack.cs's own doc comment), so
// Increment composes two Stack<int> instances via a full drain-and-rebuild, O(size)
// per call regardless of k. [GlobalSetup] builds the pushed-value workload so
// generating it is not charged to the measured replay; both arms then apply the
// same small, realistic increment window (k = 5) PushCount times, so the gap
// measured here is the honest, real price of reaching "the bottom k elements"
// through a strictly LIFO primitive instead of an indexed one.
[MemoryDiagnoser]
public class DesignAStackWithIncrementOperationBenchmarks
{
    private const int IncrementWindow = 5;
    private const int IncrementValue = 1;

    // LeetCode problem number for Design a Stack With Increment Operation.
    private const int RandomSeed = 1381;

    private const int MaxPushedValueExclusive = 1_000;

    [Params(200, 2_000)]
    public int PushCount;

    private int[] _pushedValues = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _pushedValues = Enumerable.Range(0, PushCount).Select(_ => random.Next(1, MaxPushedValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int IndexedListIncrement() =>
        Replay(DesignAStackWithIncrementOperationSolution.CreateByIndexedList(PushCount));

    [Benchmark]
    public int StackDrainAndRebuild() =>
        Replay(DesignAStackWithIncrementOperationSolution.CreateByStackDrain(PushCount));

    // Returns the top of the stack rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape LRUCacheBenchmarks/OpenTheLockBenchmarks already follow.
    private int Replay(DesignAStackWithIncrementOperationSolution.ICustomStack stack)
    {
        foreach (var value in _pushedValues)
        {
            stack.Push(value);
        }

        for (var call = 0; call < _pushedValues.Length; call++)
        {
            stack.Increment(IncrementWindow, IncrementValue);
        }

        return stack.Pop();
    }
}
