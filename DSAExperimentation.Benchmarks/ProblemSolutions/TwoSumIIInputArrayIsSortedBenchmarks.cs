using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumIIInputArrayIsSortedSolution's, the same
// methods TwoSumIIInputArrayIsSortedSolutionTests proves correct.
// TwoSumIIInputArrayIsSortedWorkloads keeps nums and the target inside LC 167's
// [-1000, 1000] and makes the only valid pair the last two elements - every
// earlier index's complement is out of the array's value range entirely, so the
// binary-search arm has to run (and fail a search) almost Length times before it
// succeeds, while the squeeze arm walks in from both ends at once rather than
// resolving on the first index.
public class TwoSumIIInputArrayIsSortedBenchmarks
{
    private int[] _nums = [];

    private int _target;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = TwoSumIIInputArrayIsSortedWorkloads.BuildNums(Length);
        _target = TwoSumIIInputArrayIsSortedWorkloads.Target;
    }

    [Benchmark(Baseline = true)]
    public int[] BinarySearch() =>
        TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByBinarySearch(_nums, _target);

    [Benchmark]
    public int[] TwoPointerSqueeze() =>
        TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByTwoPointerSqueeze(_nums, _target);
}
