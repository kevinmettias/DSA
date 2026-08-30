using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Length of Repeated Subarray (LC 718): the textbook O(n*m*min(n,m)) brute
// force that re-walks a fresh match run from every starting pair, vs. this repo's
// Memoizer running the same suffix-pair recurrence EditDistanceTests/
// DistinctSubsequencesTests already use - O(n*m) total, since each distinct (i, j)
// state's match-run length is computed once and every later reference to it (from an
// earlier diagonal, or from a neighboring row/column) reuses the cached result instead
// of rescanning it. Both arrays are identical, all-one-value arrays (the same shape as
// LeetCode's own official all-zeros example) so EVERY starting pair walks all the way
// to the end - brute force's genuine O(n^3) worst case, forced deliberately rather than
// left to chance, the same "force the real worst case" intent TwoSumBenchmarks' own
// setup comment names. A low-repeat random array would instead let brute force's early
// mismatch exit dominate and hide the asymptotic gap behind Memoizer's own per-state
// dictionary/delegate overhead.
[MemoryDiagnoser]
public class MaximumLengthOfRepeatedSubarrayBenchmarks
{
    [Params(60, 300, 1000)]
    public int Length;

    private int[] _first = null!;
    private int[] _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        _first = new int[Length];
        _second = new int[Length];
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = 0;

        for (var i = 0; i < _first.Length; i++)
        {
            for (var j = 0; j < _second.Length; j++)
            {
                var len = 0;

                while (i + len < _first.Length && j + len < _second.Length && _first[i + len] == _second[j + len])
                {
                    len++;
                }

                best = Math.Max(best, len);
            }
        }

        return best;
    }

    [Benchmark]
    public int MemoizedSuffixPairDp()
    {
        var (_, best) = Memoizer.Memoize<(int First, int Second), (int MatchLen, int Best)>((0, 0), Explore);
        return best;

        (int MatchLen, int Best) Explore((int First, int Second) state, Func<(int First, int Second), (int MatchLen, int Best)> explore)
        {
            var (i, j) = state;

            if (i == _first.Length || j == _second.Length)
            {
                return (0, 0);
            }

            var matchLen = 0;

            if (_first[i] == _second[j])
            {
                var (nextMatch, _) = explore((i + 1, j + 1));
                matchLen = 1 + nextMatch;
            }

            var (_, bestRight) = explore((i + 1, j));
            var (_, bestDown) = explore((i, j + 1));

            return (matchLen, Math.Max(matchLen, Math.Max(bestRight, bestDown)));
        }
    }
}
