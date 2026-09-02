using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Longest Valid Obstacle Course at Each Position (LC 1964): the textbook
// O(n^2) DP (dp[i] = longest non-decreasing run ending at i, rescanning every
// earlier index) vs. patience sorting via this repo's own BinarySearch.UpperBound
// over a DynamicArraySequence<int> "tails" buffer - O(n log n), the same engine
// LongestIncreasingSubsequenceBenchmarks uses with LowerBound, swapped for
// UpperBound so equal heights extend a run instead of ending it (a "valid
// course" only needs non-decreasing height, not strictly increasing).
[MemoryDiagnoser]
public class FindTheLongestValidObstacleCourseAtEachPositionBenchmarks
{
    private const int RandomSeed = 1964; // LC problem number

    [Params(2_000, 5_000)]
    public int Length;

    private int[] _obstacles = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _obstacles = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] DynamicProgramming()
    {
        var dp = new int[_obstacles.Length];

        for (var i = 0; i < _obstacles.Length; i++)
        {
            dp[i] = 1;

            for (var j = 0; j < i; j++)
            {
                if (_obstacles[j] <= _obstacles[i] && dp[j] + 1 > dp[i])
                {
                    dp[i] = dp[j] + 1;
                }
            }
        }

        return dp;
    }

    [Benchmark]
    public int[] PatienceSortingBinarySearch()
    {
        var tails = new DynamicArray<int>();
        var answer = new int[_obstacles.Length];

        for (var i = 0; i < _obstacles.Length; i++)
        {
            var position = BinarySearch.UpperBound(new DynamicArraySequence<int>(tails), _obstacles[i]);
            answer[i] = position + 1;

            if (position == tails.Count)
            {
                tails.Add(_obstacles[i]);
            }
            else
            {
                tails.Set(position, _obstacles[i]);
            }
        }

        return answer;
    }
}
