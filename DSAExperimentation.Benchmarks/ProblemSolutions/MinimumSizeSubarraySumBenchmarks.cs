using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Size Subarray Sum (LC 209): the textbook O(n) two-pointer sliding window
// needs no data structure at all - the same reason LC 11 Container With Most Water
// stayed blocked in this repo's coverage manifest. This instead compares the
// brute-force O(n^2) every-subarray scan against the O(n log n) prefix-sum +
// BinarySearch.LowerBound approach MinimumSizeSubarraySumTests uses, genuinely
// composing this repo's own BinarySearch.LowerBound over an ArraySequence<int>
// witness. Target is set one above the array's own total sum so neither strategy
// ever early-exits on a found window, forcing both through their real worst-case
// cost.
[MemoryDiagnoser]
public class MinimumSizeSubarraySumBenchmarks
{
    private const int RandomSeed = 7;
    private const int MaxElementValueExclusive = 100;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValueExclusive)).ToArray();
        _target = _nums.Sum() + 1;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var best = int.MaxValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            var sum = 0;
            for (var j = i; j < _nums.Length; j++)
            {
                sum += _nums[j];

                if (sum >= _target)
                {
                    best = Math.Min(best, j - i + 1);
                    break;
                }
            }
        }

        return best == int.MaxValue ? 0 : best;
    }

    [Benchmark]
    public int BinarySearchPrefixSum()
    {
        var prefix = new int[_nums.Length + 1];
        for (var i = 0; i < _nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + _nums[i];
        }

        var sequence = new ArraySequence<int>(prefix);
        var best = int.MaxValue;

        for (var i = 0; i < _nums.Length; i++)
        {
            var end = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, _target + prefix[i]);

            if (end <= _nums.Length)
            {
                best = Math.Min(best, end - i);
            }
        }

        return best == int.MaxValue ? 0 : best;
    }
}
