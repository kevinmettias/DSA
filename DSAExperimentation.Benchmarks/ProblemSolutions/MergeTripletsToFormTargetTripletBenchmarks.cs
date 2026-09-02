using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge Triplets to Form Target Triplet (LC 1899): an exhaustive O(2^n) subset
// search (try every subset of triplets, take its component-wise max, check
// against target) vs. the O(n) linear scan this problem actually needs - track
// which target coordinates some compatible triplet already hits exactly, via this
// repo's own Set<int>, the same membership-tracking role ContainsDuplicateBenchmarks
// already gives it. _target is deliberately unreachable so both strategies pay
// their full worst-case scan instead of an early exit making brute force look
// artificially competitive.
[MemoryDiagnoser]
public class MergeTripletsToFormTargetTripletBenchmarks
{
    private const int RandomSeed = 1899; // LC problem number
    private const int ValueBoundExclusive = 999;
    private const int ZComponentIndex = 2;
    private const int TripletDimension = 3;

    private static readonly int[] Target = [1_000, 1_000, 1_000];

    [Params(12, 18)]
    public int TripletCount;

    private int[][] _triplets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _triplets = Enumerable.Range(0, TripletCount)
            .Select(_ => new[]
            {
                random.Next(0, ValueBoundExclusive),
                random.Next(0, ValueBoundExclusive),
                random.Next(0, ValueBoundExclusive),
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceSubsets()
    {
        var count = _triplets.Length;
        for (var mask = 1; mask < (1 << count); mask++)
        {
            if (SubsetMatchesTarget(mask, count))
            {
                return true;
            }
        }

        return false;
    }

    private bool SubsetMatchesTarget(int mask, int count)
    {
        var a = 0;
        var b = 0;
        var c = 0;

        for (var i = 0; i < count; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            a = Math.Max(a, _triplets[i][0]);
            b = Math.Max(b, _triplets[i][1]);
            c = Math.Max(c, _triplets[i][ZComponentIndex]);
        }

        return a == Target[0] && b == Target[1] && c == Target[ZComponentIndex];
    }

    [Benchmark]
    public bool SetTrackedLinearScan()
    {
        var matched = new Set<int>();

        foreach (var triplet in _triplets)
        {
            if (triplet[0] > Target[0] || triplet[1] > Target[1] || triplet[ZComponentIndex] > Target[ZComponentIndex])
            {
                continue;
            }

            for (var i = 0; i < TripletDimension; i++)
            {
                if (triplet[i] == Target[i])
                {
                    matched.TryAdd(i);
                }
            }
        }

        return matched.Count == TripletDimension;
    }
}
