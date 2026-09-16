using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BinaryTreeMaximumPathSum;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeMaximumPathSum;

// Harness only. The gain recursion is BinaryTreeMaximumPathSumSolution's - this
// file pins it to LeetCode's published examples, given in LeetCode's own
// level-order-with-null array shape (BinaryTreeNode<int> is internal, so it
// cannot appear in a public TheoryData signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed partial class BinaryTreeMaximumPathSumTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            { [1, 2, 3], 6 },
            { [-10, 9, 20, null, null, 15, 7], 42 },
            { [-3], -3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPathSumByGainRecursion_LeetCodeExamples_ReturnsBestPathSum(int?[] values, int expected) =>
        Assert.Equal(expected, BinaryTreeMaximumPathSumSolution.MaxPathSumByGainRecursion(LeetCodeWireFormat.ToBinaryTree(values)));
}
