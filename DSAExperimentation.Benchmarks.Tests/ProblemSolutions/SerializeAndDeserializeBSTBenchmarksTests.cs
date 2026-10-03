using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBSTBenchmarks (ARCHITECTURE 17.9): both arms round-trip
// the same tree through their own grammar and return the rebuilt tree, so a harness whose arms
// disagree rebuilt two different trees - which BenchmarkArmsTests checks. Setup inserts the shuffled
// values 0..NodeCount-1, so the rebuilt tree must be a search tree holding exactly those values: its
// in-order walk is 0..NodeCount-1, asserted against the size Setup was asked for, which catches a
// grammar that loses, duplicates or misorders a node without needing the arms to agree on it.
public sealed partial class SerializeAndDeserializeBSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().NullMarkerQueueRoundTrip()),
            AnswerGraphText.Of(BuildHarness().NullMarkerQueueRoundTrip()));
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), InOrderValues(BuildHarness().NullMarkerQueueRoundTrip()));
    }

    [Fact]
    public void PreOrderValueOnlyRoundTrip_ShuffledBst_RebuildsEveryValueInOrder() =>
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), InOrderValues(BuildHarness().PreOrderValueOnlyRoundTrip()));

    private static SerializeAndDeserializeBSTBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    private static List<int> InOrderValues(object? root)
    {
        var values = new List<int>();
        AppendInOrder(values, (BinaryTreeNode<int>?)root);

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
