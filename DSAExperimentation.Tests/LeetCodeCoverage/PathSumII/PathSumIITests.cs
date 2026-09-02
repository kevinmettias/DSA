using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumII;

// Harness only. Both strategies are PathSumIISolution's; this file pins them to
// LeetCode's published examples. BinaryTreeNode<int> is internal, so - as in
// ValidateBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature, and each example is a private factory reused across strategies.
// Path order isn't part of LeetCode's contract ("return the paths in any order"),
// so assertions check membership and count rather than a fixed sequence.
public sealed class PathSumIITests
{
    [Fact]
    public void FindPathsByRecursiveBacktrack_ClassicExample_ReturnsBothMatchingPaths() =>
        AssertMatches([[5, 4, 11, 2], [5, 8, 4, 5]], PathSumIISolution.FindPathsByRecursiveBacktrack(ClassicTree(), 22));

    [Fact]
    public void FindPathsByAllRootToLeafPaths_ClassicExample_ReturnsBothMatchingPaths() =>
        AssertMatches([[5, 4, 11, 2], [5, 8, 4, 5]], PathSumIISolution.FindPathsByAllRootToLeafPaths(ClassicTree(), 22));

    [Fact]
    public void FindPathsByRecursiveBacktrack_NoPathSumsToTarget_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByRecursiveBacktrack(ThreeNodeTree(), 5));

    [Fact]
    public void FindPathsByAllRootToLeafPaths_NoPathSumsToTarget_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByAllRootToLeafPaths(ThreeNodeTree(), 5));

    [Fact]
    public void FindPathsByRecursiveBacktrack_SingleBranchWithNoMatch_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByRecursiveBacktrack(TwoNodeTree(), 0));

    [Fact]
    public void FindPathsByAllRootToLeafPaths_SingleBranchWithNoMatch_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByAllRootToLeafPaths(TwoNodeTree(), 0));

    [Fact]
    public void FindPathsByRecursiveBacktrack_SingleNodeTreeMatchingTarget_ReturnsSingleNodePath() =>
        AssertMatches([[5]], PathSumIISolution.FindPathsByRecursiveBacktrack(new BinaryTreeNode<int>(5), 5));

    [Fact]
    public void FindPathsByAllRootToLeafPaths_SingleNodeTreeMatchingTarget_ReturnsSingleNodePath() =>
        AssertMatches([[5]], PathSumIISolution.FindPathsByAllRootToLeafPaths(new BinaryTreeNode<int>(5), 5));

    [Fact]
    public void FindPathsByRecursiveBacktrack_EmptyTree_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByRecursiveBacktrack(null, 0));

    [Fact]
    public void FindPathsByAllRootToLeafPaths_EmptyTree_ReturnsEmptyList() =>
        Assert.Empty(PathSumIISolution.FindPathsByAllRootToLeafPaths(null, 0));

    private static void AssertMatches(int[][] expected, List<List<int>> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        foreach (var path in expected)
        {
            Assert.Contains(actual, p => p.SequenceEqual(path));
        }
    }

    // [5,4,8,11,null,13,4,7,2,null,null,5,1] - LeetCode's own example 1.
    private static BinaryTreeNode<int> ClassicTree() => new(5)
    {
        Left = new(4) { Left = new(11) { Left = new(7), Right = new(2) } },
        Right = new(8) { Left = new(13), Right = new(4) { Left = new(5), Right = new(1) } },
    };

    // [1,2,3] - LeetCode's own example 2 (targetSum 5 matches neither leaf path).
    private static BinaryTreeNode<int> ThreeNodeTree() => new(1) { Left = new(2), Right = new(3) };

    // [1,2] - LeetCode's own example 3 (targetSum 0 matches the only leaf path).
    private static BinaryTreeNode<int> TwoNodeTree() => new(1) { Left = new(2) };
}
