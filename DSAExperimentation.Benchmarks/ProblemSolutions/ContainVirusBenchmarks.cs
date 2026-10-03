using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ContainVirus;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContainVirusSolution's, the same methods
// ContainVirusSolutionTests proves correct. Each strategy clones the workload grid itself,
// so [GlobalSetup] only has to build it once per [Params] value rather than per
// iteration.
//
// LC 749 promises that each round exactly one region threatens the most uninfected
// cells. The seed is the first from LC 749's own number on whose grids at both sizes
// that holds - 749 itself ties a round at each - and ContainVirusWorkloadsTests
// replays both grids to check it.
public class ContainVirusBenchmarks
{
    private const int RandomSeed = 752;

    private int[][] _grid = [];

    [Params(10, 25)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup() => _grid = ContainVirusWorkloads.BuildGrid(Side, RandomSeed);

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill() => ContainVirusSolution.MinimumWallsByNaiveRecursiveFloodFill(_grid);

    [Benchmark]
    public int DepthFirstSearchTraversal() => ContainVirusSolution.MinimumWallsByDepthFirstSearchTraversal(_grid);
}
