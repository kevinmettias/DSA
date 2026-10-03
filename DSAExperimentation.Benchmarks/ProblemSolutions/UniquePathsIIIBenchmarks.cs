using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.UniquePathsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are UniquePathsIIISolution's, the same methods
// UniquePathsIIISolutionTests proves correct. The comparison isolates the
// constant-factor cost of Backtrack.Search's generic delegate dispatch from an
// equivalent purpose-built recursion, so both arms walk the same search tree.
//
// The grid comes from UniquePathsIIIWorkloads: Rows x 5 cells with the start, the
// end and one obstacle on seeded cells. LC 980 caps a grid at 20 cells, so Rows
// stops at 4; the search tree still grows steeply with the walkable cells, from 9
// at the smaller size to 19 at the larger.
public class UniquePathsIIIBenchmarks
{
    // LC 980's own number as the seed places the start and end where no walk covers the grid at
    // either size, which would leave a count of zero that a broken arm could match by accident; this
    // seed places them where one walk does at the smaller size and seven at the larger.
    private const int RandomSeed = 1083;
    private const int Columns = 5;
    private const int ObstacleCount = 1;

    private int[][] _grid = [];

    [Params(2, 4)]
    public int Rows { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = UniquePathsIIIWorkloads.BuildGrid(Rows, Columns, ObstacleCount, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public int SpecializedRecursive() => UniquePathsIIISolution.CountUniquePathsBySpecializedRecursion(_grid);

    [Benchmark]
    public int BacktrackEngine() => UniquePathsIIISolution.CountUniquePathsByBacktrackEngine(_grid);
}
