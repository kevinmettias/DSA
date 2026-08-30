using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Common Subsequence (LC 1143): plain bottom-up 2D array tabulation vs.
// this repo's Memoizer running the same suffix-pair recurrence EditDistanceBenchmarks/
// MaximumLengthOfRepeatedSubarrayBenchmarks already use - both O(n*m), just walking
// the table from opposite directions. Both strings are identical, all-one-character
// strings so every cell of the table is genuinely reachable and does real work, the
// same "force the real worst case" intent TwoSumBenchmarks' own setup comment names.
[MemoryDiagnoser]
public class LongestCommonSubsequenceBenchmarks
{
    [Params(60, 300)]
    public int Length;

    private string _first = null!;
    private string _second = null!;

    [GlobalSetup]
    public void Setup()
    {
        _first = new string('a', Length);
        _second = new string('a', Length);
    }

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var dp = new int[_first.Length + 1, _second.Length + 1];

        for (var i = _first.Length - 1; i >= 0; i--)
        {
            for (var j = _second.Length - 1; j >= 0; j--)
            {
                dp[i, j] = _first[i] == _second[j]
                    ? 1 + dp[i + 1, j + 1]
                    : Math.Max(dp[i + 1, j], dp[i, j + 1]);
            }
        }

        return dp[0, 0];
    }

    [Benchmark]
    public int MemoizedRecurrence()
    {
        return Memoizer.Memoize<(int First, int Second), int>((0, 0), LcsFrom);

        int LcsFrom((int First, int Second) state, Func<(int First, int Second), int> lcs)
        {
            var (i, j) = state;

            if (i == _first.Length || j == _second.Length)
            {
                return 0;
            }

            if (_first[i] == _second[j])
            {
                return 1 + lcs((i + 1, j + 1));
            }

            return Math.Max(lcs((i + 1, j)), lcs((i, j + 1)));
        }
    }
}
