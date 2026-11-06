using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.KthAncestorOfATreeNode;

// A binary-lifting table: for every node, its 2^level-th ancestor at each level, -1
// once that runs past the root. Any step count below NodeCount is a sum of distinct
// powers of two, so a kth-ancestor query is at most one jump per level instead of a
// walk of k parents. 2^Levels is the smallest power of two at least NodeCount, so
// the table holds Levels * NodeCount ints - 16 * 5 * 10^4 at LeetCode's bound,
// whatever the tree's shape.
//
// It is the binary-lifting strategy's TreeAncestor: LeetCode's constructor builds the
// table once (KthAncestorOfATreeNodeSolution.BuildAncestorJumps), and each
// getKthAncestor is answered from it. A type rather than the bare int[][] it wraps,
// since it names what the jagged array means. It answers LC 1483 and nothing else,
// so it lives beside the solution rather than in the library.
internal sealed class AncestorJumps(int[][] ancestorsByLevel, int nodeCount) : ITreeAncestor
{
    public int NodeCount => nodeCount;

    public int Levels => ancestorsByLevel.Length;

    // One jump per set bit of k, O(log k). No node is n or more steps below the
    // root, so such a step count is answered -1 before it can name a level the table
    // does not have.
    public int GetKthAncestor(int node, int generations)
    {
        if (generations >= NodeCount)
        {
            return LeetCodeAnswer.None;
        }

        var current = node;
        var remainingSteps = generations;

        for (var level = 0; remainingSteps > 0 && current != ParentArrayTree.NoParent; level++)
        {
            if ((remainingSteps & 1) == 1)
            {
                current = Jump(level, current);
            }

            remainingSteps >>= 1;
        }

        return current;
    }

    // node's 2^level-th ancestor, or -1 when node is fewer than 2^level steps below
    // the root.
    private int Jump(int level, int node) => ancestorsByLevel[level][node];
}
