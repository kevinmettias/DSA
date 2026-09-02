using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters;

// LeetCode 2246. Longest Path With Different Adjacent Characters: parent[] describes
// a tree rooted at node 0 (same prevRoom[]/CourseSchedule shape
// CountWaysToBuildRoomsInAnAntColonyTests already establishes), and the answer is
// the longest node-count path whose every adjacent pair has a different character -
// exactly TreeMetrics.Diameter's own multi-child "two tallest heights" composition,
// closed over LongestPathAlgebra's per-edge label check instead of DiameterAlgebra's
// unconditional one.
public sealed partial class LongestPathWithDifferentAdjacentCharactersTests
{
    [Fact]
    public void LongestPath_TwoValidBranchesUnderRoot_ReturnsThreeNodes()
    {
        int[] parent = [-1, 0, 0, 1, 1, 2];
        var s = "abacbe";

        var longest = LongestPath(parent, s);

        Assert.Equal(3, longest);
    }

    [Fact]
    public void LongestPath_RootWithTwoDistinctChildLabels_ReturnsAllThreeNodes()
    {
        int[] parent = [-1, 0, 0, 0];
        var s = "aabc";

        var longest = LongestPath(parent, s);

        Assert.Equal(3, longest);
    }

    [Fact]
    public void LongestPath_SingleNode_ReturnsOne()
    {
        int[] parent = [-1];
        var s = "a";

        var longest = LongestPath(parent, s);

        Assert.Equal(1, longest);
    }

    private static int LongestPath(int[] parent, string s)
    {
        var nodes = BuildTree(parent, s);

        return TreeFold.Fold<
            PathNode, PathTopology, ListChildren<PathNode>,
            NaturalChildOrder<PathNode, ListChildren<PathNode>>, ListChildren<PathNode>,
            LongestPathAlgebra, PathState>(nodes[0]).LongestPath;
    }

    private static PathNode[] BuildTree(int[] parent, string s)
    {
        var nodes = new PathNode[parent.Length];
        for (var i = 0; i < parent.Length; i++)
        {
            nodes[i] = new PathNode(i, s[i]);
        }

        for (var i = 1; i < parent.Length; i++)
        {
            nodes[parent[i]].Children.Add(nodes[i]);
        }

        return nodes;
    }
}
