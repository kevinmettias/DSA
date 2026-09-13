using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BricksFallingWhenHit;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BricksFallingWhenHitSolution's, the same methods
// BricksFallingWhenHitTests proves correct - the textbook "replay forward,
// recompute roof-connectivity by BFS after every single hit" walk, O(hits * rows
// * cols), against the reverse-time DisjointSet trick. Wall and hit-list
// construction is charged to [GlobalSetup].
[MemoryDiagnoser]
public class BricksFallingWhenHitBenchmarks
{
    // LC problem number, reused as the deterministic wall seed.
    private const int RandomSeed = 803;

    [Params(20, 60)]
    public int Size;

    private int[][] _grid = null!;
    private int[][] _hits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, seed: RandomSeed);
        _grid = wall.Grid;
        _hits = wall.Hits;
    }

    [Benchmark(Baseline = true)]
    public int[] ReplayForwardWithBfs() =>
        BricksFallingWhenHitSolution.HitBricksByForwardReplayBfs(_grid, _hits);

    [Benchmark]
    public int[] ReverseTimeDisjointSet() =>
        BricksFallingWhenHitSolution.HitBricksByReverseTimeDisjointSet(_grid, _hits);
}
