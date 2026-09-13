using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HowManyNumbersAreSmallerThanTheCurrentNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// HowManyNumbersAreSmallerThanTheCurrentNumberSolution's, the same methods
// HowManyNumbersAreSmallerThanTheCurrentNumberTests proves correct - the textbook
// O(n^2) pairwise count against sort once (this repo's own MergeSort) then
// binary-search each element's insertion point (BinarySearch.LowerBound),
// O(n log n). LeetCode's input shape is already the measured method's parameter,
// so there is nothing to hoist beyond generating the values themselves.
[MemoryDiagnoser]
public class HowManyNumbersAreSmallerThanTheCurrentNumberBenchmarks
{
    private const int RandomSeed = 1365; // LC problem number
    private const int ValueExclusiveBound = 100_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueExclusiveBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] PairwiseCount() =>
        HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentByPairwiseCount(_values);

    [Benchmark]
    public int[] SortAndLowerBound() =>
        HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentBySortAndLowerBound(_values);
}
