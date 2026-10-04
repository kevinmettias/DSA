namespace DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

// A binary-lifting table: for every node, its 2^level-th ancestor at each level, -1
// once that runs past the root. Any step count below NodeCount is a sum of distinct
// powers of two, so a kth-ancestor query is at most one jump per level instead of a
// walk of k parents. 2^Levels is the smallest power of two at least NodeCount, so
// the table holds Levels * NodeCount ints - 16 * 5 * 10^4 at LeetCode's bound,
// whatever the tree's shape.
//
// A type rather than the bare int[][] it wraps for two reasons: it names what the
// jagged array means, and it gives KthAncestorOfATreeNodeSolution's hoisted
// overload (ARCHITECTURE.md §17.4) a parameter type that can never be confused
// with the int[] parent array the LeetCode-shaped overload takes. It answers LC
// 1483 and nothing else, so it lives beside the solution rather than in Domain/.
internal sealed class AncestorJumps(int[][] ancestorsByLevel, int nodeCount)
{
    public int NodeCount => nodeCount;

    public int Levels => ancestorsByLevel.Length;

    // node's 2^level-th ancestor, or -1 when node is fewer than 2^level steps below
    // the root.
    public int Jump(int level, int node) => ancestorsByLevel[level][node];
}
