using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Compares ShortestPath.Dijkstra (THeuristic = ZeroHeuristic) against
// ShortestPath.AStar (THeuristic = ManhattanHeuristic) on the same open grid and
// the same source/target pair - per ARCHITECTURE.md, "AStar is Dijkstra closed
// over a heuristic," so this is one algorithm with the injected axis swapped, not
// two competing implementations. On an open (wall-free) grid the Manhattan
// heuristic is exact, so AStar should settle far fewer nodes than Dijkstra's full
// expanding frontier before reaching the far corner.
[MemoryDiagnoser]
public class ShortestPathHeuristicBenchmarks
{
    [Params(20, 80)]
    public int GridSize;

    private WeightedGridNode _source = null!;
    private WeightedGridNode _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (source, farCorner) = WeightedGridWorkloads.OpenGrid(GridSize);
        _source = source;
        _target = farCorner;
    }

    [Benchmark(Baseline = true)]
    public int Dijkstra()
        => ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(_source)[_target];

    [Benchmark]
    public int AStar()
        => ShortestPath.AStar<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int, ManhattanHeuristic>(
            _source, _target)!.Value;
}
