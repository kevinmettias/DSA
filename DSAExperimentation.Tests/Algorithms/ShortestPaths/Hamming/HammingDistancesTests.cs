using DSAExperimentation.Algorithms.ShortestPaths.Hamming;
using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths.Hamming;

public sealed class HammingDistancesTests
{
    [Fact]
    public void From_LabelsTheRootWithZero()
    {
        var graph = HammingGraph.Build("aa", ["ab"]);

        var distances = HammingDistances.From(graph.Root);

        Assert.Equal(0, distances[graph.Root]);
    }

    [Fact]
    public void From_LabelsEveryReachableNodeWithItsEdgeCount()
    {
        var graph = HammingGraph.Build("hit", ["hot", "dot", "dog", "cog"]);

        AssertDistanceTo(graph, "cog", 4);
    }

    [Fact]
    public void From_OmitsNodesInOtherComponents()
    {
        // "zzz" shares no one-character edge with the "aaa" component.
        var graph = HammingGraph.Build("aaa", ["aab", "zzz"]);

        var distances = HammingDistances.From(graph.Root);
        var found = graph.TryGetNode("zzz", out var isolated);

        Assert.True(found);
        Assert.False(distances.ContainsKey(isolated));
    }

    [Fact]
    public void From_ReportsBreadthFirstDistanceNotDiscoveryOrder()
    {
        // Both "ab" and "ba" sit one step from "aa"; "bb" sits two away via either.
        var graph = HammingGraph.Build("aa", ["ab", "ba", "bb"]);

        AssertDistanceTo(graph, "bb", 2);
    }

    // The named node has to come back at the given number of one-character edits: the BFS
    // edge count from the root, not the order in which the walk discovered it.
    private static void AssertDistanceTo(HammingGraph graph, string label, int expected)
    {
        var distances = HammingDistances.From(graph.Root);
        var found = graph.TryGetNode(label, out var node);

        Assert.True(found);
        Assert.Equal(expected, distances[node]);
    }
}
