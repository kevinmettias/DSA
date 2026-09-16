using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.FindElementsInAContaminatedBinaryTree;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindElementsInAContaminatedBinaryTree;

// LeetCode 1261. Find Elements in a Contaminated Binary Tree. See
// FindElementsInAContaminatedBinaryTreeSolution for the two strategies: a recursive
// recovery into a BCL List answered by linear scan, and this repo's own
// TopDownTraversal recovering into a Set<int> answered in O(1).
//
// Examples are stated as LeetCode's own level-order arrays - BinaryTreeNode<int> is
// internal, so it cannot appear in a public TheoryData member; LeetCodeWireFormat.ToBinaryTree
// reconstructs the tree inside each test method instead. Every present node carries
// the contaminated value -1 and null stands for a missing child: only the shape is
// input, since recovery overwrites every value.
public sealed partial class FindElementsInAContaminatedBinaryTreeTests
{
    public static TheoryData<int?[], int[], bool[]> Examples =>
        new()
        {
            // LeetCode example 1: root -> right only, recovering to {0, 2}.
            { [-1, null, -1], [1, 2], [false, true] },

            // LeetCode example 2, widened to the full 0..6 sweep the pre-migration
            // test asserted: a five-node complete tree recovering to {0, 1, 2, 3, 4}.
            { [-1, -1, -1, -1, -1], [0, 1, 2, 3, 4, 5, 6], [true, true, true, true, true, false, false] },

            // LeetCode example 3: a left-leaning chain hanging off the right child,
            // recovering to {0, 2, 5, 11} - the case that proves values are derived
            // from position rather than from insertion order.
            { [-1, null, -1, -1, null, -1], [2, 3, 4, 5], [true, false, false, true] },

            // Root alone: 0 is present and nothing else is.
            { [-1], [0, 1], [true, false] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByListScan_LeetCodeExamples_FindsExactlyTheRecoveredValues(
        int?[] levelOrder, int[] targets, bool[] expected) =>
        AssertFinds(
            FindElementsInAContaminatedBinaryTreeSolution.CreateByListScan(LeetCodeWireFormat.ToBinaryTree(levelOrder)!),
            targets,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByTopDownSet_LeetCodeExamples_FindsExactlyTheRecoveredValues(
        int?[] levelOrder, int[] targets, bool[] expected) =>
        AssertFinds(
            FindElementsInAContaminatedBinaryTreeSolution.CreateByTopDownSet(LeetCodeWireFormat.ToBinaryTree(levelOrder)!),
            targets,
            expected);

    private static void AssertFinds(IFindElements elements, int[] targets, bool[] expected)
    {
        for (var i = 0; i < targets.Length; i++)
        {
            Assert.Equal(expected[i], elements.Find(targets[i]));
        }
    }
}
