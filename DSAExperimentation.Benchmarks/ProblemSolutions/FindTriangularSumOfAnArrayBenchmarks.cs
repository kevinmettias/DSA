using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTriangularSumOfAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTriangularSumOfAnArraySolution's, the same methods
// FindTriangularSumOfAnArrayTests proves correct. No closed-form shortcut is
// composable from this repo's primitives, so both run the identical O(n^2) pairwise
// reduction - a raw in-place int[] buffer against this repo's own DynamicArray<int>
// rebuilt fresh each round - isolating the primitive's own overhead rather than
// comparing two different algorithms.
[MemoryDiagnoser]
public class FindTriangularSumOfAnArrayBenchmarks
{
    private const int RandomSeed = 2221; // LC problem number
    private const int DigitBound = 10; // LC 2221's inputs are single decimal digits

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InPlaceArrayReduction() => FindTriangularSumOfAnArraySolution.TriangularSumByInPlaceArray(_nums);

    [Benchmark]
    public int DynamicArrayReduction() => FindTriangularSumOfAnArraySolution.TriangularSumByDynamicArray(_nums);
}
