using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;

// Shape under test: a NESTED collection answer, where both the row order and each
// row's own order are significant - the opposite of K Closest Points to Origin,
// and the reason LeetCodeAnswers makes you name which one you mean.
internal sealed class BinaryTreeLevelOrderTraversalRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<int?[], List<List<int>>>("binary-tree-level-order-traversal")
            .Strategy(
                "QueueLevels",
                levelOrder => BinaryTreeLevelOrderTraversalSolution.LevelOrderByQueueLevels(
                    LeetCodeWireFormat.ToBinaryTree(levelOrder)))
            .Strategy(
                "LevelGroupedTraversal",
                levelOrder => BinaryTreeLevelOrderTraversalSolution.LevelOrderByLevelGroupedTraversal(
                    LeetCodeWireFormat.ToBinaryTree(levelOrder)))
            .MatchingAnswersWith(LeetCodeAnswers.IsSequenceOfSequencesEqual<int>)
            .Case("example-1", [3, 9, 20, null, null, 15, 7], [[3], [9, 20], [15, 7]])
            .Case("example-2", [1], [[1]])
            .Case("empty-tree", [], [])
            .Case("left-spine-only", [1, 2, null, 3], [[1], [2], [3]])
            .Workload("perfect-depth-12", BuildPerfectLevelOrder(depth: 12))
            .Build();

    private static int?[] BuildPerfectLevelOrder(int depth)
        => [.. Enumerable.Range(1, (1 << depth) - 1).Select(value => (int?)value)];
}
