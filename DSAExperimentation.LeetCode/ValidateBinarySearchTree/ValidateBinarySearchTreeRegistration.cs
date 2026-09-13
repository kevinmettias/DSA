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
            .MatchingAnswersWith(LeetCodeAnswers.Exactly)
            .Case("example-1", [2, 1, 3], true)
            .Case("example-2", [5, 1, 4, null, null, 3, 6], false)
            .Case("empty-tree", [], true)
            .Case("single-node", [1], true)

            // The case a naive "left child < parent < right child" check passes and
            // a real bounds check does not: 3 is below its own parent 4, but it
            // sits in 5's right subtree.
            .Case("violates-only-a-distant-ancestor", [5, 4, 6, null, null, 3, 7], false)
            .Build();
}
