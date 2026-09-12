using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindModeInBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindModeInBinarySearchTree;

// Harness only. Both mode-finding strategies are
// FindModeInBinarySearchTreeSolution's; this file pins them to LeetCode's
// published examples. BinaryTreeNode<int> is internal, so - as in
// RecoverBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature; each example is a private factory rebuilt fresh per [Fact].
// LeetCode accepts the modes in any order, so both strategies' results are
// sorted before comparing - the hash-map strategy in particular has no reason to
// come out in ascending order the way the in-order-walk strategy naturally does.
public sealed class FindModeInBinarySearchTreeTests
{
    [Fact]
    public void FindModeByHashMapFrequencyCount_SingleModeFromADuplicateInARightSubtree_ReturnsThatValue() =>
        Assert.Equal(
            [2],
            FindModeInBinarySearchTreeSolution
                .FindModeByHashMapFrequencyCount(SingleModeInRightSubtree())
                .OrderBy(x => x));

    [Fact]
    public void FindModeByInOrderTraversalStreak_SingleModeFromADuplicateInARightSubtree_ReturnsThatValue() =>
        Assert.Equal(
            [2],
            FindModeInBinarySearchTreeSolution
                .FindModeByInOrderTraversalStreak(SingleModeInRightSubtree())
                .OrderBy(x => x));

    [Fact]
    public void FindModeByHashMapFrequencyCount_MultipleValuesTiedForMostFrequent_ReturnsAllOfThem() =>
        Assert.Equal(
            [1, 3],
            FindModeInBinarySearchTreeSolution
                .FindModeByHashMapFrequencyCount(TiedModesAcrossBothSubtrees())
                .OrderBy(x => x));

    [Fact]
    public void FindModeByInOrderTraversalStreak_MultipleValuesTiedForMostFrequent_ReturnsAllOfThem() =>
        Assert.Equal(
            [1, 3],
            FindModeInBinarySearchTreeSolution
                .FindModeByInOrderTraversalStreak(TiedModesAcrossBothSubtrees())
                .OrderBy(x => x));

    [Fact]
    public void FindModeByHashMapFrequencyCount_SingleNode_ReturnsItsValue() =>
        Assert.Equal(
            [7],
            FindModeInBinarySearchTreeSolution
                .FindModeByHashMapFrequencyCount(new BinaryTreeNode<int>(7))
                .OrderBy(x => x));

    [Fact]
    public void FindModeByInOrderTraversalStreak_SingleNode_ReturnsItsValue() =>
        Assert.Equal(
            [7],
            FindModeInBinarySearchTreeSolution
                .FindModeByInOrderTraversalStreak(new BinaryTreeNode<int>(7))
                .OrderBy(x => x));

    // [1,null,2,2] -> [2]
    private static BinaryTreeNode<int> SingleModeInRightSubtree() =>
        new(1) { Right = new(2) { Left = new(2) } };

    // [2,1,3,1,null,3] -> [1,3], one tie in each subtree
    private static BinaryTreeNode<int> TiedModesAcrossBothSubtrees() =>
        new(2)
        {
            Left = new(1) { Left = new(1) },
            Right = new(3) { Right = new(3) },
        };
}
