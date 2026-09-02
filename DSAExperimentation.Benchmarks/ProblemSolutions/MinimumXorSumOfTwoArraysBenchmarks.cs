using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum XOR Sum of Two Arrays (LC 1879): the textbook unmemoized bitmask
// recursion over (index, assignedMask) - the same (index, mask) state re-explored
// from scratch down every branch, since many different assignment orderings reach
// an identical "these nums2 elements are already used" state - vs. the same
// recursion routed through this repo's own Memoizer, the identical (int, int)
// tuple-state shape MinimumCostToConnectTwoGroupsOfPointsBenchmarks already uses
// for LC 1595's near-twin "index, connected-mask" assignment recurrence.
[MemoryDiagnoser]
public class MinimumXorSumOfTwoArraysBenchmarks
{
    // 1879 is the LC problem number.
    private const int RandomSeed = 1879;
    private const int MaxValueBitWidth = 16;

    [Params(4, 7)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1 << MaxValueBitWidth)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1 << MaxValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => MinXorSum(0, 0);

    private int MinXorSum(int index, int mask)
    {
        if (index == Length)
        {
            return 0;
        }

        var best = int.MaxValue;
        for (var j = 0; j < Length; j++)
        {
            if ((mask & (1 << j)) != 0)
            {
                continue;
            }

            var candidate = (_nums1[index] ^ _nums2[j]) + MinXorSum(index + 1, mask | (1 << j));
            best = Math.Min(best, candidate);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Index, int Mask), int>((0, 0), (state, costFor) =>
    {
        var (index, mask) = state;

        if (index == Length)
        {
            return 0;
        }

        var best = int.MaxValue;
        for (var j = 0; j < Length; j++)
        {
            if ((mask & (1 << j)) != 0)
            {
                continue;
            }

            var candidate = (_nums1[index] ^ _nums2[j]) + costFor((index + 1, mask | (1 << j)));
            best = Math.Min(best, candidate);
        }

        return best;
    });
}
