using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfSquarefulArrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfSquarefulArraysSolution's, the same methods
// NumberOfSquarefulArraysTests proves correct - generate every distinct permutation
// and filter at the leaves, against Backtrack.Search with the perfect-square
// adjacency folded into candidate enumeration so an invalid prefix is abandoned
// immediately.
[MemoryDiagnoser]
public class NumberOfSquarefulArraysBenchmarks
{
    private const int ValueUpperBound = 50;

    // Fixed so every run measures the same values.
    private const int ValueSeed = 1;

    private int[] _nums = [];

    [Params(8, 10)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(ValueSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBound)).ToArray();
        Array.Sort(_nums);
    }

    [Benchmark(Baseline = true)]
    public int GenerateThenFilter() =>
        NumberOfSquarefulArraysSolution.CountSquarefulPermsByFullPermutationFilter(_nums);

    [Benchmark]
    public int PrunedBacktrack() =>
        NumberOfSquarefulArraysSolution.CountSquarefulPermsByPrunedBacktracking(_nums);
}
