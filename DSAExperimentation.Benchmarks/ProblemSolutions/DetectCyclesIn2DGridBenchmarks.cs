using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DetectCyclesIn2DGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DetectCyclesIn2DGridSolution's - the hand-rolled
// parent-tracked DFS baseline against this repo's own DisjointSet edge-union
// strategy. A uniformly random three-letter grid is dense enough in same-character
// adjacencies that both arms find a cycle early at either size, so the measurement
// is about how fast each one gets there.
[MemoryDiagnoser]
public class DetectCyclesIn2DGridBenchmarks
{
    // LC problem number, reused as the deterministic seed.
    private const int RandomSeed = 1559;

    private static readonly char[] Letters = ['a', 'b', 'c'];

    private char[][] _grid = [];

    [Params(30, 150)]
    public int GridSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new char[GridSize][];

        for (var r = 0; r < GridSize; r++)
        {
            _grid[r] = new char[GridSize];
            for (var c = 0; c < GridSize; c++)
            {
                _grid[r][c] = Letters[random.Next(Letters.Length)];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool ParentTrackedDepthFirstSearch() =>
        DetectCyclesIn2DGridSolution.ContainsCycleByParentTrackedDepthFirstSearch(_grid);

    [Benchmark]
    public bool DisjointSetEdgeUnion() =>
        DetectCyclesIn2DGridSolution.ContainsCycleByDisjointSetEdgeUnion(_grid);
}
