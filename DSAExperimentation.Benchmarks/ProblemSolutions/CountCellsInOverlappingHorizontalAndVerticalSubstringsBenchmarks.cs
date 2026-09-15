using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountCellsInOverlappingHorizontalAndVerticalSubstrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution's, the same
// methods CountCellsInOverlappingHorizontalAndVerticalSubstringsTests proves
// correct.
[MemoryDiagnoser]
public class CountCellsInOverlappingHorizontalAndVerticalSubstringsBenchmarks
{
    private const int Seed = 3529;

    private char[][] _grid = [];

    private string _pattern = "";
    [Params(60, 300)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => (_grid, _pattern) = OverlappingSubstringGridWorkloads.Build(GridSize, Seed);

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution.CountCellsByBruteForce(_grid, _pattern);

    [Benchmark]
    public int ZFunction() =>
        CountCellsInOverlappingHorizontalAndVerticalSubstringsSolution.CountCellsByZFunction(_grid, _pattern);
}
