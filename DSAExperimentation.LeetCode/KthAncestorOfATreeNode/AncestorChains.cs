namespace DSAExperimentation.LeetCode.KthAncestorOfATreeNode;

// Every node's full root-to-parent ancestor chain, in root-first order, indexed by
// node id: chain[^k] is that node's kth ancestor. Precomputed once so each
// getKthAncestor call is an O(1) index instead of an O(k) walk.
//
// A type rather than the bare int[][] it wraps for two reasons: it names what the
// jagged array means, and it gives KthAncestorOfATreeNodeSolution's hoisted
// overload (ARCHITECTURE.md §17.4) a parameter type that can never be confused
// with the int[] parent array the LeetCode-shaped overload takes. It answers LC
// 1483 and nothing else, so it lives beside the solution rather than in Domain/.
internal sealed class AncestorChains(int[][] chainsById)
{
    public int Count => chainsById.Length;

    // The chain for one node: root first, immediate parent last, empty at the root.
    public int[] Of(int node) => chainsById[node];
}
