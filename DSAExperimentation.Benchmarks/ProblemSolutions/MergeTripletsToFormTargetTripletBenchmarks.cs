using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MergeTripletsToFormTargetTriplet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeTripletsToFormTargetTripletSolution's, the same
// methods MergeTripletsToFormTargetTripletTests proves correct - an exhaustive
// O(2^n) subset search against the O(n) Set<int>-tracked linear scan. Target is
// deliberately unreachable so both strategies pay their full worst-case scan
// instead of an early exit making brute force look artificially competitive. The
// triplet array is LeetCode's own input shape, so neither arm needs a hoisted
// overload.
[MemoryDiagnoser]
public class MergeTripletsToFormTargetTripletBenchmarks
{
    private const int RandomSeed = 1899; // LC problem number
    private const int ValueBoundExclusive = 999;
    private const int TripletDimension = 3;

    private static readonly int[] Target = [1_000, 1_000, 1_000];

    private int[][] _triplets = [];

    [Params(12, 18)]
    public int TripletCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _triplets = Enumerable.Range(0, TripletCount)
            .Select(_ => Enumerable.Range(0, TripletDimension)
                .Select(_ => random.Next(0, ValueBoundExclusive))
                .ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceSubsets() =>
        MergeTripletsToFormTargetTripletSolution.CanFormTargetByBruteForceSubsets(_triplets, Target);

    [Benchmark]
    public bool SetTrackedLinearScan() =>
        MergeTripletsToFormTargetTripletSolution.CanFormTargetBySetTrackedLinearScan(_triplets, Target);
}
