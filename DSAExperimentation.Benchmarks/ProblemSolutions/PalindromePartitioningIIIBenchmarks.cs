using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Palindrome Partitioning III (LC 1278): the naive doubly-recursive partition
// search (PalindromePartitioningIIITests' recurrence, called with no cache -
// FibonacciNumberBenchmarks precedent) vs. this repo's Memoizer-backed
// top-down DP over the same (Position, PartitionsLeft) state. Both explore
// the same decision tree; NaiveRecursion re-solves every (position,
// partitionsLeft) pair as many times as it's reached along different choice
// paths, while MemoizedTopDown solves each one exactly once.
[MemoryDiagnoser]
public class PalindromePartitioningIIIBenchmarks
{
    private const int Unreachable = int.MaxValue / 2;

    [Params(12, 18)]
    public int Length;

    private string _s = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1278);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(4))).ToArray());
        _k = Length / 2;
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => MinChanges(0, _k);

    private int MinChanges(int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == _s.Length ? 0 : Unreachable;
        }

        if (position == _s.Length)
        {
            return Unreachable;
        }

        var best = Unreachable;
        var lastEnd = _s.Length - (partitionsLeft - 1);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            var candidate = ChangesToPalindrome(position, end - 1) + MinChanges(end, partitionsLeft - 1);
            best = Math.Min(best, candidate);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedTopDown()
        => Memoizer.Memoize<(int Position, int PartitionsLeft), int>((0, _k), (state, changesFrom) =>
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == _s.Length ? 0 : Unreachable;
            }

            if (position == _s.Length)
            {
                return Unreachable;
            }

            var best = Unreachable;
            var lastEnd = _s.Length - (partitionsLeft - 1);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                var candidate = ChangesToPalindrome(position, end - 1) + changesFrom((end, partitionsLeft - 1));
                best = Math.Min(best, candidate);
            }

            return best;
        });

    private int ChangesToPalindrome(int l, int r)
    {
        var changes = 0;

        while (l < r)
        {
            if (_s[l++] != _s[r--])
            {
                changes++;
            }
        }

        return changes;
    }
}
