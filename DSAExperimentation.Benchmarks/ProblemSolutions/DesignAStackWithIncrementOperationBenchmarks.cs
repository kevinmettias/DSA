using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design a Stack With Increment Operation (LC 1381): an array-backed stack with
// direct indexed access increments only the bottom min(k, size) slots directly,
// O(k) per call - vs. this repo's own Stack<int>, whose public surface is
// deliberately LIFO-only (Push/TryPop/TryPeek, no indexer - see Stack.cs's own
// doc comment), so DesignAStackWithIncrementOperationTests' Increment composes two
// Stack<int> instances via a full drain-and-rebuild, O(size) per call regardless
// of k. Both benchmarks apply the same small, realistic increment window (k = 5)
// PushCount times, so the gap measured here is the honest, real price of reaching
// "the bottom k elements" through a strictly LIFO primitive instead of an indexed
// one.
[MemoryDiagnoser]
public class DesignAStackWithIncrementOperationBenchmarks
{
    private const int IncrementWindow = 5;
    private const int IncrementValue = 1;

    [Params(200, 2_000)]
    public int PushCount;

    private int[] _pushedValues = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1381);
        _pushedValues = Enumerable.Range(0, PushCount).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArrayBackedIndexedIncrement()
    {
        var values = new List<int>(_pushedValues);

        for (var call = 0; call < values.Count; call++)
        {
            var affected = Math.Min(IncrementWindow, values.Count);

            for (var i = 0; i < affected; i++)
            {
                values[i] += IncrementValue;
            }
        }

        return values[^1];
    }

    [Benchmark]
    public int RepoStackDrainAndRebuild()
    {
        var stack = new RepoIntStack();

        foreach (var value in _pushedValues)
        {
            stack.Push(value);
        }

        for (var call = 0; call < _pushedValues.Length; call++)
        {
            Increment(stack, IncrementWindow, IncrementValue);
        }

        stack.TryPeek(out var top);
        return top;
    }

    private static void Increment(RepoIntStack stack, int k, int val)
    {
        var scratch = new RepoIntStack();

        while (stack.TryPop(out var item))
        {
            scratch.Push(item);
        }

        var affected = Math.Min(k, scratch.Count);

        for (var i = 0; i < affected; i++)
        {
            scratch.TryPop(out var item);
            stack.Push(item + val);
        }

        while (scratch.TryPop(out var item))
        {
            stack.Push(item);
        }
    }
}
