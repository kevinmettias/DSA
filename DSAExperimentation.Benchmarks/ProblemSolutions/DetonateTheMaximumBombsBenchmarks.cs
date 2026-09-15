using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DetonateTheMaximumBombs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DetonateTheMaximumBombsSolution's, the same methods
// DetonateTheMaximumBombsTests proves correct - the hand-rolled bool[] recursion
// against the same successor closure run through this repo's own
// DepthFirstSearch.Traverse. Both are handed LeetCode's own input shape, generated
// once in [GlobalSetup]. The radius range is deliberately wide enough relative to
// the grid that blasts genuinely chain, so both arms walk real components rather
// than bouncing off isolated bombs.
[MemoryDiagnoser]
public class DetonateTheMaximumBombsBenchmarks
{
    private const int GridSize = 1_000;
    private const int MaxRadius = 60;
    private const int RandomSeed = 2101;

    private int[][] _bombs = [];

    [Params(50, 300)]
    public int BombCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _bombs = new int[BombCount][];

        for (var i = 0; i < BombCount; i++)
        {
            _bombs[i] = [random.Next(0, GridSize), random.Next(0, GridSize), random.Next(1, MaxRadius)];
        }
    }

    [Benchmark(Baseline = true)]
    public int ManualRecursiveVisit() =>
        DetonateTheMaximumBombsSolution.MaxDetonationsByManualRecursion(_bombs);

    [Benchmark]
    public int RepoDepthFirstSearch() =>
        DetonateTheMaximumBombsSolution.MaxDetonationsByRepoDepthFirstSearch(_bombs);
}
