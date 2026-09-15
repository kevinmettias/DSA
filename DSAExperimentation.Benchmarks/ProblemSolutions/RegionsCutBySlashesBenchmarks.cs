using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RegionsCutBySlashes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RegionsCutBySlashesSolution's, the same methods
// RegionsCutBySlashesTests proves correct. The classic 3x3-subgrid expansion
// (baseline) blows each cell up into nine squares and flood-fills the resulting
// 9n^2-cell grid with an explicit stack; the composed arm unions four triangles
// per cell in this repo's own DisjointSet and counts distinct roots with this
// repo's own Set<int> - the same Union-Find primitive NumberOfProvincesBenchmarks
// already measures against a DFS flood fill, on a finer-grained triangle graph.
[MemoryDiagnoser]
public class RegionsCutBySlashesBenchmarks
{
    private const int RandomSeed = 1;

    private string[] _grid = [];

    [Params(30, 150)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = RegionsCutBySlashesWorkloads.BuildGrid(GridSize, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int ThreeByThreeExpansionFloodFill() =>
        RegionsCutBySlashesSolution.CountRegionsByExpandedFloodFill(_grid);

    [Benchmark]
    public int DisjointSetTriangleUnion() =>
        RegionsCutBySlashesSolution.CountRegionsByDisjointSetTriangles(_grid);
}
