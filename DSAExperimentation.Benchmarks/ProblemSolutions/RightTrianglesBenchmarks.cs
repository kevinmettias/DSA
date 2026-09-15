using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RightTriangles;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RightTrianglesSolution's, the same methods
// RightTrianglesTests proves correct. Neither strategy needs input construction
// beyond the grid itself, so there is no hoisted overload here - matching
// TypeOfTriangleBenchmarks' own precedent for a problem whose input is already in
// its measured shape.
[MemoryDiagnoser]
public class RightTrianglesBenchmarks
{
    private const int Seed = 3128;

    private int[][] _grid = [];

    [Params(20, 200)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = RightTriangleWorkloads.BuildGrid(Size, seed: Seed);

    [Benchmark(Baseline = true)]
    public long BruteForceRowColumnScan() => RightTrianglesSolution.CountByBruteForceRowColumnScan(_grid);

    [Benchmark]
    public long TalliedRowsAndColumns() => RightTrianglesSolution.CountByTalliedRowsAndColumns(_grid);
}
