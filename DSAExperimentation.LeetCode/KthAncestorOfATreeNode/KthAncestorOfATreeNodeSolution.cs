using System.Numerics;

namespace DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

// LeetCode 1483. Kth Ancestor of a Tree Node: given a rooted tree as a parent
// array, answer repeated "what is node's kth ancestor" queries, reporting -1 when
// the chain runs out before the requested number of steps. Up to 5 * 10^4 nodes
// and 5 * 10^4 queries, with k up to n - so a single tree can be one straight
// chain, where a walk of k parents costs up to 5 * 10^4 steps per query.
//
// Every Graph-domain topology contract here (ITreeTopology included) is child-ward
// only - GetChildren, never GetParent - so the faster strategy cannot walk ancestors
// live off a topology and precomputes them from the parent array instead.
internal static class KthAncestorOfATreeNodeSolution
{
    // Marks the root in LeetCode's parent-array encoding, and the answer once a
    // walk steps off the top of the tree.
    private const int NoParent = LeetCodeAnswer.None;

    // The textbook baseline this precompute has to justify itself against: step
    // up the raw parent array one step at a time, O(k) per query and no precompute
    // at all. Deliberately written without this repo's primitives.
    public static int GetKthAncestorByParentWalk(int[] parent, int node, int ancestorSteps)
    {
        var current = node;

        for (var step = 0; step < ancestorSteps; step++)
        {
            if (current == NoParent)
            {
                return LeetCodeAnswer.None;
            }

            current = parent[current];
        }

        return current;
    }

    // Binary lifting, in LeetCode's own shape: one query against a table this call
    // builds. A caller answering many queries builds the table once with
    // BuildAncestorJumps and uses the overload below.
    public static int GetKthAncestorByBinaryLifting(int[] parent, int node, int ancestorSteps) =>
        GetKthAncestorByBinaryLifting(BuildAncestorJumps(parent), node, ancestorSteps);

    // The hoisted overload (ARCHITECTURE.md §17.4): the table is already built, so
    // all that is charged here is one jump per set bit of ancestorSteps, O(log k).
    // No node is n or more steps below the root, so such a step count is answered
    // -1 before it can name a level the table does not have.
    public static int GetKthAncestorByBinaryLifting(AncestorJumps jumps, int node, int ancestorSteps)
    {
        if (ancestorSteps >= jumps.NodeCount)
        {
            return LeetCodeAnswer.None;
        }

        var current = node;
        var remainingSteps = ancestorSteps;

        for (var level = 0; remainingSteps > 0 && current != NoParent; level++)
        {
            if ((remainingSteps & 1) == 1)
            {
                current = jumps.Jump(level, current);
            }

            remainingSteps >>= 1;
        }

        return current;
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

        if (midway == NoParent)
        {
            return NoParent;
        }

        return jumps[midway];
    }
}
