using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.ValidateBinarySearchTree;

// Shape under test: a TREE input stated as LeetCode's level-order array, and a
// problem with only ONE strategy. The single strategy matters as much as the tree
// does - the harness must not assume a problem is a comparison between arms, or
// every problem that has just one right answer falls out of it.
internal sealed class ValidateBinarySearchTreeRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<int?[], bool>("validate-binary-search-tree")
            .Strategy(
                "BoundsRecursion",
                levelOrder => ValidateBinarySearchTreeSolution.IsValidByBoundsRecursion(
                    LeetCodeWireFormat.ToBinaryTree(levelOrder)))
            .MatchingAnswersWith(LeetCodeAnswers.IsExactlyEqual)
            .Case("example-1", [2, 1, 3], true)
            .Case("example-2", [5, 1, 4, null, null, 3, 6], false)
            .Case("empty-tree", [], true)
            .Case("single-node", [1], true)

            // The case a naive "left child < parent < right child" check passes and
            // a real bounds check does not: 3 is below its own parent 4, but it
            // sits in 5's right subtree.
            .Case("violates-only-a-distant-ancestor", [5, 4, 6, null, null, 3, 7], false)

            // The retired per-problem benchmark measured a three-node tree, which
            // is small enough that the measurement was dominated by call overhead
            // and said nothing about the traversal. A tree deep enough to matter
            // replaces it - and a VALID one, so the walk cannot exit early and is
            // charged for every node.
            .Workload("perfect-depth-14", BuildPerfectSearchTree(depth: 14))
            .Build();

    // Level-order for a perfect BST holding 1..(2^depth - 1): the node at
    // level-order index i takes the midpoint of the value range its subtree owns,
    // which is what makes the in-order walk come out ascending.
    private static int?[] BuildPerfectSearchTree(int depth)
    {
        var levelOrder = new int?[(1 << depth) - 1];

        FillLevelOrder(index: 0, lowest: 1, highest: levelOrder.Length);

        return levelOrder;

        void FillLevelOrder(int index, int lowest, int highest)
        {
            if (index >= levelOrder.Length || lowest > highest)
            {
                return;
            }

            var middle = lowest + ((highest - lowest) / 2);
            levelOrder[index] = middle;

            FillLevelOrder((2 * index) + 1, lowest, middle - 1);
            FillLevelOrder((2 * index) + 2, middle + 1, highest);
        }
    }
}
