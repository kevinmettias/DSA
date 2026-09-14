using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfGroupsWithIncreasingLength;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfGroupsWithIncreasingLengthSolution's,
// the same methods MaximumNumberOfGroupsWithIncreasingLengthTests proves correct.
// The greedy sweep the two share is linear, so what is being measured is the sort in
// front of it - the textbook O(n^2) insertion sort (HeightCheckerBenchmarks'
// precedent) against this repo's own MergeSort over ArrayIndexedSequence at
// O(n log n). [GlobalSetup] draws the limits, so only the solve is charged.
[MemoryDiagnoser]
public class MaximumNumberOfGroupsWithIncreasingLengthBenchmarks
{
    private const int RandomSeed = 2790; // LC problem number
    private const int UsageLimitUpperBoundExclusive = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _usageLimits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _usageLimits = Enumerable.Range(0, Length).Select(_ => random.Next(1, UsageLimitUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InsertionSortThenGreedySweep() =>
        MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByInsertionSort(_usageLimits);

    [Benchmark]
    public int MergeSortThenGreedySweep() =>
        MaximumNumberOfGroupsWithIncreasingLengthSolution.MaxIncreasingGroupsByMergeSort(_usageLimits);
}
