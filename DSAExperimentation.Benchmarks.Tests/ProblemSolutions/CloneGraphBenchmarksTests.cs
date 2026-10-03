using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.CloneGraph;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CloneGraphBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: that the
// clone is the ring Setup built, known from Setup's construction rather than from either arm. Both arms return the
// cloned root as object? (the node type is internal to the solution tier, CS0050), so the tests walk it.
public sealed partial class CloneGraphBenchmarksTests
{
    private const int SmallestNodeCount = 10;

    // Setup numbers its ring's nodes 1..NodeCount in index order and returns nodes[0], so the graph's
    // root carries value 1 and every other value is reachable from it.
    private const int RingRootValue = 1;

    [Fact]
    public void DictionaryDfs_TenNodeRing_ClonesTheRingFromValueOne() =>
        AssertClonesTheRing(BuildHarness().DictionaryDfs());

    [Fact]
    public void HashMapDfs_TenNodeRing_ClonesTheRingFromValueOne() =>
        AssertClonesTheRing(BuildHarness().HashMapDfs());

    private static void AssertClonesTheRing(object? answer)
    {
        var root = Assert.IsType<Node>(answer);

        Assert.Equal(RingRootValue, root.Value);
        Assert.Equal(Enumerable.Range(RingRootValue, SmallestNodeCount), ReachableValues(root).Order());
    }

    // Each node reachable from the root, once, by reference - a record's value equality would merge nodes.
    private static IEnumerable<int> ReachableValues(Node root)
    {
        var seen = new HashSet<Node>(ReferenceEqualityComparer.Instance) { root };
        var pending = new Queue<Node>([root]);

        while (pending.TryDequeue(out var node))
        {
            yield return node.Value;

            foreach (var neighbor in node.Neighbors.Where(seen.Add))
            {
                pending.Enqueue(neighbor);
            }
        }
    }

    private static CloneGraphBenchmarks BuildHarness()
    {
        var harness = new CloneGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
