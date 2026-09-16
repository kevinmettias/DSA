using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.PathSumIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumIII;

// Harness only. Both strategies are PathSumIIISolution's - this file pins them
// to LeetCode's published examples, given in LeetCode's own level-order-with-
// null array shape (BinaryTreeNode<int> is internal, so it cannot appear in a
// public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed partial class PathSumIIITests
{
    public static TheoryData<int?[], int, int> Examples =>
        new()
        {
            // 10
            // |-- 5
            // |   |-- 3
            // |   |   |-- 3
            // |   |   `-- -2
            // |   `-- 2
            // |       `-- 1
            // `-- -3
            //     `-- 11
            // Matching paths (sum 8): 5->3, 5->2->1, -3->11.
            { [10, 5, -3, 3, 2, null, 11, 3, -2, null, 1], 8, 3 },
            { [1, -2, -3], 100, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathSumByDoubleDfs_LeetCodeExamples_ReturnsMatchingPathCount(
        int?[] values, int target, int expected)
    {
        var pathCount = PathSumIIISolution.PathSumByDoubleDfs(LeetCodeWireFormat.ToBinaryTree(values), target);

        Assert.Equal(expected, pathCount);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PathSumByPrefixSumHashMap_LeetCodeExamples_ReturnsMatchingPathCount(
        int?[] values, int target, int expected)
    {
        var pathCount = PathSumIIISolution.PathSumByPrefixSumHashMap(LeetCodeWireFormat.ToBinaryTree(values), target);

        Assert.Equal(expected, pathCount);
    }
}
