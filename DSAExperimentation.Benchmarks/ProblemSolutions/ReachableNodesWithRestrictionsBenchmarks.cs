using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.ReachableNodesWithRestrictions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReachableNodesWithRestrictionsSolution's, the same
// methods ReachableNodesWithRestrictionsTests proves correct. Each is handed the
// prepared restriction Set its hoisted overload takes, so building the membership
// structure is charged to [GlobalSetup] rather than to the count being measured -
// leaving the flood fill's adjacency copy against the union-find's edge pass as the
// only difference the numbers report.
[MemoryDiagnoser]
public class ReachableNodesWithRestrictionsBenchmarks
{
    // LC problem number, reused as the deterministic tree seed.
    private const int RandomSeed = 2368;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[][] _edges = null!;
    private Set<int> _restricted = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (edges, restricted) = RestrictedTreeWorkloads.Build(NodeCount, RandomSeed);
        _edges = edges;
        _restricted = new Set<int>(restricted);
    }

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill() =>
        ReachableNodesWithRestrictionsSolution.ReachableNodesByDepthFirstFloodFill(
            NodeCount, _edges, _restricted);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        ReachableNodesWithRestrictionsSolution.ReachableNodesByDisjointSet(NodeCount, _edges, _restricted);
}
