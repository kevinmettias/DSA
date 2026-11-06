using System.Numerics;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.KthAncestorOfATreeNode;

// LeetCode 1483. Kth Ancestor of a Tree Node: given a rooted tree as a parent
// array, answer repeated "what is node's kth ancestor" queries, reporting -1 when
// the chain runs out before the requested number of steps. Up to 5 * 10^4 nodes
// and 5 * 10^4 queries, with k up to n - so a single tree can be one straight
// chain, where a walk of k parents costs up to 5 * 10^4 steps per query.
//
// Every Graph-domain topology contract here (ITreeTopology included) is child-ward
// only - GetChildren, never GetParent - so the faster strategy cannot walk ancestors
// live off a topology and precomputes them from the parent array instead.
//
// This is a design problem - LeetCode's own shape is a TreeAncestor built from
// (n, parent) and answering one getKthAncestor(node, k) at a time - so each strategy
// is a factory taking LeetCode's constructor inputs and returning an ITreeAncestor,
// the object the catalog harness replays a call script on (ARCHITECTURE.md §17.13).
[LeetCodeQuestion("kth-ancestor-of-a-tree-node", LeetCodeAnswerOrder.AsWritten)]
internal static class KthAncestorOfATreeNodeSolution
{
    // The textbook baseline this precompute has to justify itself against: step
    // up the raw parent array one step at a time, O(k) per query and no precompute
    // at all. LeetCode's constructor gives n beside the parent array it is the
    // length of, so neither strategy reads it.
    public static ITreeAncestor CreateByParentWalk(int nodeCount, int[] parent) => new ParentWalkAncestor(parent);

    // Binary lifting: the constructor builds the table once, and each query is one
    // jump per set bit of k, O(log k).
    public static ITreeAncestor CreateByBinaryLifting(int nodeCount, int[] parent) => BuildAncestorJumps(parent);

    // Deliberately written without this repo's primitives.
    private sealed class ParentWalkAncestor(int[] parent) : ITreeAncestor
    {
        public int GetKthAncestor(int node, int generations)
        {
            var current = node;

            for (var step = 0; step < generations; step++)
            {
                if (current == ParentArrayTree.NoParent)
                {
                    return LeetCodeAnswer.None;
                }

                current = parent[current];
            }

            return current;
        }
    }

    // Level 0 is the parent array itself; each level above jumps twice along the one
    // below, so a node's 2^level-th ancestor is its 2^(level-1)-th ancestor's own.
    // Built a whole level at a time, every jump a level reads is already in place
    // whatever order the parent array numbers its nodes in - no traversal of the
    // tree, so a chain of 5 * 10^4 nodes costs no deeper a stack than a star.
    public static AncestorJumps BuildAncestorJumps(int[] parent)
    {
        var levels = LevelsFor(parent.Length);
        var ancestorsByLevel = new int[levels][];

        for (var level = 0; level < levels; level++)
        {
            ancestorsByLevel[level] = level == 0 ? CopyOf(parent) : DoubledJumps(ancestorsByLevel[level - 1]);
        }

        return new AncestorJumps(ancestorsByLevel, parent.Length);
    }

    // A tree of nodeCount nodes is at most nodeCount - 1 steps deep, and the
    // smallest power of two at least nodeCount, 2^levels, exceeds every such step
    // count - so levels 0..levels-1 spell each of them in binary.
    private static int LevelsFor(int nodeCount)
    {
        var firstStepCountPastEveryLevel = BitOperations.RoundUpToPowerOf2((uint)nodeCount);

        return BitOperations.Log2(firstStepCountPastEveryLevel);
    }

    private static int[] CopyOf(int[] parent) => (int[])parent.Clone();

    private static int[] DoubledJumps(int[] jumps)
    {
        var doubled = new int[jumps.Length];

        for (var node = 0; node < jumps.Length; node++)
        {
            doubled[node] = JumpTwice(jumps, node);
        }

        return doubled;
    }

    private static int JumpTwice(int[] jumps, int node)
    {
        var midway = jumps[node];

        if (midway == ParentArrayTree.NoParent)
        {
            return ParentArrayTree.NoParent;
        }

        return jumps[midway];
    }
}
