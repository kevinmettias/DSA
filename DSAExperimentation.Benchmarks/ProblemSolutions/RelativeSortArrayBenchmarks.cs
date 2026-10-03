using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RelativeSortArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RelativeSortArraySolution's, the same methods
// RelativeSortArraySolutionTests proves correct. RelativeSortArrayWorkloads builds the
// arrays: _arr1 holds every _arr2 value, and the rest of it is half values drawn from
// _arr2 (exercises the ranked branch) and half values past _arr2's range (exercises the
// unranked, sort-by-value-ascending branch and forces every LinearScanComparerSort miss
// through a full m-length scan). LC 1122 caps both arrays at 1,000 values from
// [0, 1000], so Length stops at 1,000 and ReferenceLength (m) is 100, leaving the values
// from 200 to 1,000 for the unranked half - the O(n log n * m) scan cost against
// HashMap's O(1) lookups, at the sizes LeetCode poses.
public class RelativeSortArrayBenchmarks
{
    private const int ReferenceLength = 100;
    private const int RandomSeed = 1122; // LC problem number

    private int[] _arr1 = [];

    private int[] _arr2 = [];
    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => (_arr1, _arr2) = RelativeSortArrayWorkloads.Build(Length, ReferenceLength, RandomSeed);

    [Benchmark(Baseline = true)]
    public int[] LinearScanComparerSort() =>
        RelativeSortArraySolution.RelativeSortByLinearScanComparer(_arr1, _arr2);

    [Benchmark]
    public int[] HashMapMergeSort() =>
        RelativeSortArraySolution.RelativeSortByHashMapMergeSort(_arr1, _arr2);
}
