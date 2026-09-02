using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Increasing Quadruplets (LC 2552): the textbook O(n^4) four-nested-loop scan
// vs. this repo's own FenwickTree<int,SumOperation<int>> - a Binary Indexed Tree of
// suffix values inserted as k sweeps right to left - answering "how many l>k have
// nums[l]>nums[j]" as an O(log n) range-count query instead of a fourth nested loop
// (CountIncreasingQuadrupletsTests' own approach). Params stay small on purpose:
// the O(n^4) baseline would otherwise dominate the run, so both arms are measured
// on the same modest permutation sizes rather than letting the baseline set an
// unreasonably tiny Length just for itself.
[MemoryDiagnoser]
public class CountIncreasingQuadrupletsBenchmarks
{
    private const int RandomSeed = 2552; // LeetCode problem number

    [Params(8, 16)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var n = _nums.Length;
        var count = 0L;

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                for (var k = j + 1; k < n; k++)
                {
                    for (var l = k + 1; l < n; l++)
                    {
                        if (_nums[i] < _nums[k] && _nums[k] < _nums[j] && _nums[j] < _nums[l])
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    [Benchmark]
    public long FenwickTreeSweep()
    {
        var n = _nums.Length;
        var suffixGreaterCounts = new FenwickTree<int, SumOperation<int>>(n);
        var total = 0L;

        for (var k = n - 1; k >= 0; k--)
        {
            var leftSmallerCount = 0;

            for (var j = 0; j < k; j++)
            {
                if (_nums[j] > _nums[k])
                {
                    var rightGreaterCount = _nums[j] == n ? 0 : suffixGreaterCounts.Query(_nums[j], n - 1);
                    total += (long)leftSmallerCount * rightGreaterCount;
                }
                else
                {
                    leftSmallerCount++;
                }
            }

            suffixGreaterCounts.Add(_nums[k] - 1, 1);
        }

        return total;
    }
}
