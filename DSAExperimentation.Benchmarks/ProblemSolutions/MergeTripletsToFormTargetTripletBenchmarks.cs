using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MergeTripletsToFormTargetTriplet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeTripletsToFormTargetTripletSolution's, the same
// methods MergeTripletsToFormTargetTripletSolutionTests proves correct - an exhaustive
// O(2^n) subset search against the O(n) Set<int>-tracked linear scan. Target is
// deliberately unreachable so both strategies pay their full worst-case scan
// instead of an early exit making brute force look artificially competitive. The
// triplet array is LeetCode's own input shape, so neither arm needs a hoisted
// overload.
//
// Sizes are per arm. The subset search stops at 18 triplets; the linear scan runs on
// to LC 1899's own bound of 100,000, where its O(n) growth can show, and the two are
// compared at the sizes both run.
public class MergeTripletsToFormTargetTripletBenchmarks
{
    private const int RandomSeed = 1899; // LC problem number
    private const int ValueBoundExclusive = 999;
    private const int TripletDimension = 3;

    private static readonly int[] Target = [1_000, 1_000, 1_000];

    private Dictionary<int, int[][]> _tripletsBySize = [];

    public static IEnumerable<int> BaselineSizes => [12, 18];

    public static IEnumerable<int> LinearScanSizes => [.. BaselineSizes, 1_000, 100_000];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _tripletsBySize = LinearScanSizes.ToDictionary(tripletCount => tripletCount, BuildTriplets);

    private static int[][] BuildTriplets(int tripletCount)
    {
        var random = new Random(RandomSeed);

        return Enumerable.Range(0, tripletCount)
            .Select(_ => SeededDraws.Values(TripletDimension, 0, ValueBoundExclusive, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool CanFormTargetByBruteForceSubsets(int tripletCount) =>
        MergeTripletsToFormTargetTripletSolution.CanFormTargetByBruteForceSubsets(
            _tripletsBySize[tripletCount], Target);

    [Benchmark]
    [ArgumentsSource(nameof(LinearScanSizes))]
    public bool CanFormTargetBySetTrackedLinearScan(int tripletCount) =>
        MergeTripletsToFormTargetTripletSolution.CanFormTargetBySetTrackedLinearScan(
            _tripletsBySize[tripletCount], Target);
}
