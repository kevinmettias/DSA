using DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindMinimumInRotatedSortedArraySolution's, the
// same methods FindMinimumInRotatedSortedArraySolutionTests proves correct. [GlobalSetup]
// builds one ascending run of distinct values and rotates it at a fixed non-zero
// pivot, so both arms search the same rotated array. This backfills the
// compile-smoke placeholder the manifest recorded for this problem.
public class FindMinimumInRotatedSortedArrayBenchmarks
{
    private const int RandomSeed = 153; private int[] _nums = [];

    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var pivot = random.Next(1, Length);
        _nums = Enumerable.Range(0, Length).Select(i => (i + pivot) % Length).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => FindMinimumInRotatedSortedArraySolution.FindMinByLinearScan(_nums);

    [Benchmark]
    public int PredicateSearch() => FindMinimumInRotatedSortedArraySolution.FindMinByPredicateSearch(_nums);
}
