using BenchmarkDotNet.Attributes;

using RepoLongStack = DSAExperimentation.DataStructures.Stack.Stack<long>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Replace Non-Coprime Numbers in Array (LC 2197): repeatedly sweeping the whole
// remaining list from the start for a mergeable adjacent pair (restarting the scan
// after every single merge, since a merge can cascade further back) against this
// repo's own Stack<long> holding only the merged prefix, where cascading is just
// "keep peeking/popping the top" - no rescan ever needed because the stack's top is
// always the most recently finalized element.
[MemoryDiagnoser]
public class ReplaceNonCoprimeNumbersInArrayBenchmarks
{
    private const int RandomSeed = 2197;

    // Small value range so adjacent numbers frequently share a factor, forcing
    // real merge (and merge-cascade) work on every run.
    private const int MinValueInclusive = 2;
    private const int MaxValueExclusive = 10;

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedFullRescan()
    {
        var list = _values.Select(v => (long)v).ToList();
        var mergedAny = true;

        while (mergedAny)
        {
            mergedAny = MergeFirstNonCoprimePair(list);
        }

        return list.Count;
    }

    private static bool MergeFirstNonCoprimePair(List<long> list)
    {
        for (var i = 0; i < list.Count - 1; i++)
        {
            var gcd = Gcd(list[i], list[i + 1]);

            if (gcd > 1)
            {
                list[i] = list[i] / gcd * list[i + 1];
                list.RemoveAt(i + 1);
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public int StackCascadingMerge()
    {
        var stack = new RepoLongStack();

        foreach (var num in _values)
        {
            long current = num;

            while (stack.TryPeek(out var top) && Gcd(top, current) > 1)
            {
                stack.TryPop(out _);
                current = current / Gcd(top, current) * top;
            }

            stack.Push(current);
        }

        return stack.Count;
    }

    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
}
