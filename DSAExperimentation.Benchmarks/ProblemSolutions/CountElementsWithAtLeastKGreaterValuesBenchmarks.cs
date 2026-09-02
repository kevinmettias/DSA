using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.CountElementsWithAtLeastKGreaterValues;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountElementsWithAtLeastKGreaterValuesSolution's,
// the same methods CountElementsWithAtLeastKGreaterValuesTests proves correct.
// Sorting is charged to [GlobalSetup] via the SortedUpperBound arm's hoisted
// overload, so only the per-element bisection loop is measured - the same
// "hoist construction, keep the search measured" split OpenTheLockBenchmarks
// uses for LockGraph.Build.
[MemoryDiagnoser]
public class CountElementsWithAtLeastKGreaterValuesBenchmarks
{
    private const int Seed = 3759;
    private const int MaxValueExclusive = 1_000_000;
    private const int K = 10;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private ArraySequence<int> _sortedNums;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();

        var sorted = (int[])_nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        _sortedNums = new ArraySequence<int>(sorted);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => CountElementsWithAtLeastKGreaterValuesSolution.CountQualifiedByBruteForce(_nums, K);

    [Benchmark]
    public int SortedUpperBound() =>
        CountElementsWithAtLeastKGreaterValuesSolution.CountQualifiedBySortedUpperBound(_sortedNums, K);
}
