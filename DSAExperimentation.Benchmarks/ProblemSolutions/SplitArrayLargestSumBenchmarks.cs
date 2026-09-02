using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Split Array Largest Sum (LC 410): a hand-rolled int lo/hi bisection loop vs.
// this repo's own BinarySearch.LowerBound over an on-demand FeasibleSplitSequence
// (SplitArrayLargestSumTests precedent) - both binary-search the same monotone
// feasibility predicate in O(nums.Length * log(sum - max)), just one through a
// bespoke loop and the other through the reusable IRandomAccessSequence<bool>
// abstraction.
[MemoryDiagnoser]
public class SplitArrayLargestSumBenchmarks
{
    private const int RandomSeed = 410; // LC problem number
    private const int MaxElementValue = 1_000; // exclusive upper bound passed to Random.Next
    private const int KDivisor = 20; // number of splits k derived as a fraction of Length
    private const int MidpointDivisor = 2; // classic binary search midpoint

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
        _k = Math.Max(1, Length / KDivisor);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch()
    {
        var low = _nums.Max();
        var high = _nums.Sum();

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);
            if (CanSplitWithinLimit(_nums, _k, mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var floor = _nums.Max();
        var ceiling = _nums.Sum();
        var sequence = new FeasibleSplitSequence(_nums, _k, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

    private static bool CanSplitWithinLimit(int[] nums, int k, int limit)
    {
        var subarrays = 1;
        var currentSum = 0;

        foreach (var num in nums)
        {
            if (currentSum + num > limit)
            {
                subarrays++;
                currentSum = 0;
            }

            currentSum += num;
        }

        return subarrays <= k;
    }

    private readonly struct FeasibleSplitSequence(int[] nums, int k, int floor, int ceiling) : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanSplitWithinLimit(nums, k, floor + index);
    }
}
