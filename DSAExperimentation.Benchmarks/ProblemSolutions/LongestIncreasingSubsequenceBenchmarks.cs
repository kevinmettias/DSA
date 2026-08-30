using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Increasing Subsequence (LC 300): the textbook O(n^2) DP (dp[i] = longest
// run ending at i, rescanning every earlier index) vs. patience sorting via this
// repo's BinarySearch.LowerBound over a DynamicArraySequence<int> view of a
// DynamicArray<int> "tails" buffer - O(n log n), the same LowerBound engine
// SearchInsertPositionTests already exercises, composed here with DynamicArray
// instead of a fixed array so the sequence's Length can grow as the tails run
// extends.
[MemoryDiagnoser]
public class LongestIncreasingSubsequenceBenchmarks
{
    [Params(2_000, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DynamicProgramming()
    {
        var dp = new int[_values.Length];
        var best = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (_values[j] < _values[i] && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }

            best = Math.Max(best, dp[i]);
        }

        return best;
    }

    [Benchmark]
    public int PatienceSortingBinarySearch()
    {
        var tails = new DynamicArray<int>();

        foreach (var num in _values)
        {
            var position = BinarySearch.LowerBound(new DynamicArraySequence<int>(tails), num);

            if (position == tails.Count)
            {
                tails.Add(num);
            }
            else
            {
                tails.Set(position, num);
            }
        }

        return tails.Count;
    }
}
