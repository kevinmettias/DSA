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

        var distances = HammingDistances.From(graph.Root);

        Assert.True(graph.TryGetNode("cog", out var cog));
        Assert.Equal(4, distances[cog]);
    }

    [Fact]
    public void From_OmitsNodesInOtherComponents()
    {
        // "zzz" shares no one-character edge with the "aaa" component.
        var graph = HammingGraph.Build("aaa", ["aab", "zzz"]);

        var distances = HammingDistances.From(graph.Root);

        Assert.True(graph.TryGetNode("zzz", out var isolated));
        Assert.False(distances.ContainsKey(isolated));
    }

    [Fact]
    public void From_ReportsBreadthFirstDistanceNotDiscoveryOrder()
    {
        // Both "ab" and "ba" sit one step from "aa"; "bb" sits two away via either.
        var graph = HammingGraph.Build("aa", ["ab", "ba", "bb"]);

        var distances = HammingDistances.From(graph.Root);

        Assert.True(graph.TryGetNode("bb", out var far));
        Assert.Equal(2, distances[far]);
    }
}
