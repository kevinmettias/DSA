using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBSTBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's rebuilt tree to a property of the workload's
// shape. Setup inserts the shuffled values 0..NodeCount-1, so the rebuilt tree must be a search tree
// holding exactly those values: its in-order walk is 0..NodeCount-1, asserted against the size Setup
// was asked for, which catches a grammar that loses, duplicates or misorders a node without needing
// the arms to agree on it.
public sealed partial class SerializeAndDeserializeBSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void NullMarkerQueueRoundTrip_ShuffledBst_RebuildsEveryValueInOrder() =>
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), InOrderValues(BuildHarness().NullMarkerQueueRoundTrip()));

    private static SerializeAndDeserializeBSTBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static List<int> InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendInOrder(values, root);

        return values;
    }

    private static void AppendInOrder(List<int> values, BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(values, node.Left);
        values.Add(node.Value);
        AppendInOrder(values, node.Right);
    }
}
