using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.HowManyNumbersAreSmallerThanTheCurrentNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// HowManyNumbersAreSmallerThanTheCurrentNumberSolution's, the same methods
// HowManyNumbersAreSmallerThanTheCurrentNumberSolutionTests proves correct - the textbook
// O(n^2) pairwise count against sort once (this repo's own MergeSort) then
// binary-search each element's insertion point (BinarySearch.LowerBound),
// O(n log n). LeetCode's input shape is already the measured method's parameter,
// so there is nothing to hoist beyond generating the values themselves. Length stops
// at LC 1365's 500-element cap and values are drawn from its [0, 100].
public class HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks
{
    private const int RandomSeed = 1365; // LC problem number
    private const int ValueExclusiveBound = 101;

    private int[] _values = [];

    [Params(200, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, 0, ValueExclusiveBound, random);
    }

    [Benchmark(Baseline = true)]
    public int[] PairwiseCount() =>
        HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentByPairwiseCount(_values);

    [Benchmark]
    public int[] SortAndLowerBound() =>
        HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentBySortAndLowerBound(_values);
}
