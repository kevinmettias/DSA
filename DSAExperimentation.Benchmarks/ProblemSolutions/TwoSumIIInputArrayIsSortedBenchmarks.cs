using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TwoSumIIInputArrayIsSorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is TwoSumIIInputArrayIsSortedSolution's, the same
// method TwoSumIIInputArrayIsSortedTests proves correct. nums is 0..Length-1 with
// target chosen so the only valid pair is the last two elements - every earlier
// index's complement is out of the array's value range entirely, so the outer
// loop has to run (and fail a binary search) almost Length times before it
// succeeds, rather than resolving on the first index.
[MemoryDiagnoser]
public class TwoSumIIInputArrayIsSortedBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _target;

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

    [Benchmark]
    public int[] BinarySearch() =>
        TwoSumIIInputArrayIsSortedSolution.TryFindIndicesByBinarySearch(_nums, _target);
}
