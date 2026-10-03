using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostOfAPathWithSpecialRoadsSolution's, the same
// methods MinimumCostOfAPathWithSpecialRoadsSolutionTests proves correct. [GlobalSetup] takes
// the start, target and random special-road list MinimumCostOfAPathWithSpecialRoadsWorkloads
// draws inside the square LC 2662 allows between them - already LeetCode's own argument
// shape, so no hoisted overload is needed - and each arm then builds its own view of the
// point graph, which is part of what that strategy costs. The graph is dense (every
// pair of the ~2*SpecialRoadCount+2 points is directly connected), so the array-scan
// baseline is expected to win: the well-known dense-graph case where a heap's O(log V)
// bookkeeping costs more than it saves.
public class MinimumCostOfAPathWithSpecialRoadsBenchmarks
{
    private const int Seed = 2662; // LC problem number
    private const int CoordinateUpperBound = 1_000;
    private const int CostUpperBound = 500;

    private int[] _start = [];

    private int[] _target = [];
    private int[][] _specialRoads = [];
    [Params(20, 100)]
    public int SpecialRoadCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_start, _target, _specialRoads) = MinimumCostOfAPathWithSpecialRoadsWorkloads.Build(
            SpecialRoadCount, CoordinateUpperBound, CostUpperBound, Seed);

    [Benchmark(Baseline = true)]
    public int ArrayDijkstra() =>
        MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByArrayScanDijkstra(_start, _target, _specialRoads);

    [Benchmark]
    public int RepoHeapDijkstra() =>
        MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByHeapDijkstra(_start, _target, _specialRoads);
}
