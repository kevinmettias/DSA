using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.CountWaysToBuildRoomsInAnAntColony;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountWaysToBuildRoomsInAnAntColonySolution's, the
// same methods CountWaysToBuildRoomsInAnAntColonyTests proves agree.
//
// The workload is a straight chain, where the per-node algebra is at its worst:
// every node's subtree size is close to n, so recomputing Factorial(size) from
// scratch at each node is real O(n) work, O(n^2) total - exactly the case the
// precomputed O(1)-per-lookup table is for. A random earlier-parent tree stays
// O(log n) deep on average, which would hide the difference instead of showing it.
[MemoryDiagnoser]
public class CountWaysToBuildRoomsInAnAntColonyBenchmarks
{
    private RootedTreeNode _root = null!;

    [Params(200, 2_000)]
    public int RoomCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = ParentArrayTree.Chain(RoomCount);

    [Benchmark(Baseline = true)]
    public int PerNodeFactorials() =>
        CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPerNodeFactorial(_root);

    [Benchmark]
    public int PrecomputedFactorials() =>
        CountWaysToBuildRoomsInAnAntColonySolution.WaysToBuildOrderByPrecomputedFactorials(_root, RoomCount);
}
