using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Palindrome Partitioning IV (LC 1745): plain unmemoized recursive
// backtracking over the same (Position, PartitionsLeft) state space vs. this
// repo's Memoizer caching it (PalindromePartitioningIII precedent). _s is
// "a"*BlockSize + "b" + "a"*BlockSize + "c": the trailing "c" forces the
// third partition to be exactly "c" (it's the only "c" in the string), which
// leaves "a"*BlockSize + "b" + "a"*BlockSize needing a 2-way palindrome
// split that provably has none (a valid split would require the two "a"
// runs flanking "b" to have equal length on both sides of every candidate
// cut, which never happens) - so CheckPartitioning always resolves to false
// only after exhausting every candidate, forcing both strategies through
// their full worst-case search instead of an early exit on the first
// invocation making the naive version look artificially competitive. Many
// different first/second cut choices land on the same later (Position, 1)
// state, so the naive version re-explores it once per incoming path while
// the memoized version resolves it exactly once.
[MemoryDiagnoser]
public class PalindromePartitioningIVBenchmarks
{
    private const string MiddleSeparator = "b";
    private const string UniqueTrailingCharacter = "c";
    private const int PartitionCount = 3;

    [Params(30, 100)]
    public int BlockSize;

    private string _s = null!;

    [GlobalSetup]
    public void Setup() => _s = new string('a', BlockSize) + MiddleSeparator + new string('a', BlockSize) + UniqueTrailingCharacter;

    [Benchmark(Baseline = true)]
    public bool Unmemoized() => CanSplit(0, PartitionCount);

    private bool CanSplit(int position, int partitionsLeft)
    {
        if (partitionsLeft == 0)
        {
            return position == _s.Length;
        }

        var lastEnd = _s.Length - (partitionsLeft - 1);

        for (var end = position + 1; end <= lastEnd; end++)
        {
            if (IsPalindrome(position, end - 1) && CanSplit(end, partitionsLeft - 1))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool Memoized()
        => Memoizer.Memoize<(int Position, int PartitionsLeft), bool>((0, PartitionCount), (state, canSplit) =>
        {
            var (position, partitionsLeft) = state;

            if (partitionsLeft == 0)
            {
                return position == _s.Length;
            }

            var lastEnd = _s.Length - (partitionsLeft - 1);

            for (var end = position + 1; end <= lastEnd; end++)
            {
                if (IsPalindrome(position, end - 1) && canSplit((end, partitionsLeft - 1)))
                {
                    return true;
                }
            }

            return false;
        });

    private bool IsPalindrome(int l, int r)
    {
        while (l < r)
        {
            if (_s[l++] != _s[r--])
            {
                return false;
            }
        }

        return true;
    }
}
