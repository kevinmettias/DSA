using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumIIInputArrayIsSortedSolution's, the same
// methods TwoSumIIInputArrayIsSortedTests proves correct. nums is 0..Length-1
// with target chosen so the only valid pair is the last two elements - every
// earlier index's complement is out of the array's value range entirely, so the
// binary-search arm has to run (and fail a search) almost Length times before it
// succeeds, while the squeeze arm walks in from both ends at once rather than
// resolving on the first index.
[MemoryDiagnoser]
public class TwoSumIIInputArrayIsSortedBenchmarks
{
    private int[] _nums = [];

    private int _target;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _nums[i] = i;
        }

        _target = (2 * Length) - 3; // uniquely nums[Length - 2] + nums[Length - 1]
    }

    [Benchmark(Baseline = true)]
    public int[] BinarySearch() =>
        TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByBinarySearch(_nums, _target);

    [Benchmark]
    public int[] TwoPointerSqueeze() =>
        TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByTwoPointerSqueeze(_nums, _target);
}
