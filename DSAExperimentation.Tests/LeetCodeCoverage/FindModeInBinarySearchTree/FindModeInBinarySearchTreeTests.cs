using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindModeInBinarySearchTree;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindModeInBinarySearchTree;

// Harness only. Both mode-finding strategies are
// FindModeInBinarySearchTreeSolution's; this file pins them to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape.
// BinaryTreeNode<int> is internal, so - as in BinaryTreeLevelOrderTraversalTests -
// it stays out of a public TheoryData signature and LeetCodeWireFormat.ToBinaryTree reconstructs each
// example fresh per theory row (CS0053 is why this file used one [Fact] per example
// before). LeetCode accepts the modes in any order, so both strategies' results are
// sorted before comparing - the hash-map strategy in particular has no reason to
// come out in ascending order the way the in-order-walk strategy naturally does.
public sealed partial class FindModeInBinarySearchTreeTests
{
    public static TheoryData<TreeExample> Examples =>
        new()
        {
            // [1,null,2,2] -> [2]
            { new TreeExample([1, null, 2, 2], [2]) },

            // [2,1,3,1,null,null,3] -> [1,3], one tie in each subtree
            // (LeetCode writes it [2,1,3,1,null,3], eliding the trailing nulls).
            { new TreeExample([2, 1, 3, 1, null, null, 3], [1, 3]) },

            { new TreeExample([7], [7]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindModeByHashMapFrequencyCount_LeetCodeExamples_ReturnsEveryMostFrequentValue(
        TreeExample example) =>
        Assert.Equal(
            example.Expected,
            FindModeInBinarySearchTreeSolution
                .FindModeByHashMapFrequencyCount(LeetCodeWireFormat.ToBinaryTree(example.Values))
                .OrderBy(x => x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindModeByInOrderTraversalStreak_LeetCodeExamples_ReturnsEveryMostFrequentValue(
        TreeExample example) =>
        Assert.Equal(
            example.Expected,
            FindModeInBinarySearchTreeSolution
                .FindModeByInOrderTraversalStreak(LeetCodeWireFormat.ToBinaryTree(example.Values))
                .OrderBy(x => x));

    public readonly record struct TreeExample(int?[] Values, int[] Expected);
}
