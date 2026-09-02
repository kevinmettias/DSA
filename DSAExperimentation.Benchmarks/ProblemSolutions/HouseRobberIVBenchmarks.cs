using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// House Robber IV (LC 2560): a linear scan of every candidate capability from
// min(nums) upward (baseline) vs. this repo's own BinarySearch.LowerBound over an
// on-demand FeasibleCapabilitySequence (HouseRobberIVTests precedent, the same
// "binary search on the answer" shape KokoEatingBananasBenchmarks already uses) -
// both binary-search the same monotone feasibility predicate, just one through an
// O(range * n) sweep and the other through an O(n * log(range)) bisection.
[MemoryDiagnoser]
public class HouseRobberIVBenchmarks
{
    private const int RandomSeed = 2560; // LeetCode problem number
    private const int MaxValueExclusive = 2_000;
    private const int KDivisor = 4;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _k = (Length / KDivisor) + 1;
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var floor = _nums.Min();
        var ceiling = _nums.Max();

        for (var cap = floor; cap <= ceiling; cap++)
        {
            if (CanRobAtLeastKWithinCap(_nums, _k, cap))
            {
                return cap;
            }
        }

        return ceiling;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var floor = _nums.Min();
        var ceiling = _nums.Max();
        var sequence = new FeasibleCapabilitySequence(_nums, _k, floor, ceiling);

        return floor + BinarySearch.LowerBound(sequence, true);
    }

    private static bool CanRobAtLeastKWithinCap(int[] nums, int k, int cap)
    {
        var count = 0;
        var previousRobbed = false;

        foreach (var value in nums)
        {
            if (value <= cap && !previousRobbed)
            {
                count++;
                previousRobbed = true;
            }
            else
            {
                previousRobbed = false;
            }
        }

        return count >= k;
    }

    private readonly struct FeasibleCapabilitySequence(int[] nums, int k, int floor, int ceiling) : IRandomAccessSequence<bool>
    {
        public int Length => ceiling - floor + 1;

        public bool Get(int index) => CanRobAtLeastKWithinCap(nums, k, floor + index);
    }
}
