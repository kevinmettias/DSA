using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.OperationsOnTree;

// LeetCode 1993. Operations on Tree: lock/unlock/upgrade over a tree given as a
// parent array, where upgrade(num, user) succeeds only if num is unlocked, no
// ancestor of num is locked, and at least one descendant is - releasing every one
// of those descendants as it takes the lock.
//
// This is a design problem - LeetCode's own shape is a stateful object with three
// operations, not a single return value - so each strategy is a factory handing
// back a LockingTree, the same shape LRUCacheSolution and MinStackSolution use for
// their own design problems (LC 146, LC 155). Lock, Unlock and the locked-ancestor
// walk are strategy-independent and live on LockingTree itself; the two arms differ
// only in how upgrade finds the locked nodes beneath its target.
//
// Ancestor checks are a plain walk up the parent array rather than a composed
// Graph-domain query, because the Graph topology contracts here are deliberately
// child-ward only (GetChildren, never GetParent - see KthAncestorOfATreeNode), so
// there is no witness for that direction to compose in the first place.
internal static class OperationsOnTreeSolution
{
    // The textbook answer: no child links at all, just the parent array. To find
    // what sits beneath a node, test every node in the whole tree by walking its
    // own ancestor chain - O(NodeCount * depth) per query. Deliberately BCL-only
    // inside (§17.5); it is the arm the subtree walk below has to justify itself
    // against, and until this migration it lived only in the benchmark, where
    // nothing ever asserted it.
    public static LockingTree CreateByWholeTreeScan(int[] parent) => new WholeTreeScanLockingTree(parent);

    // This repo's own DepthFirstSearch.Traverse (ARCHITECTURE.md §12's bare-
    // successor-function engine) over the RootedTreeNode tree DataStructures'
    // ParentArrayTree materializes, with successors = node => node.Children. It
    // only ever visits the target's actual subtree, so the cost is proportional to
    // that subtree rather than to the whole tree.
    public static LockingTree CreateBySubtreeDepthFirstSearch(int[] parent) =>
        new SubtreeDepthFirstSearchLockingTree(parent);

    private sealed class WholeTreeScanLockingTree(int[] parent) : LockingTree(parent)
    {
        public override int[] LockedDescendantsOf(int num)
        {
            var found = new List<int>();

            for (var candidate = 0; candidate < NodeCount; candidate++)
            {
                if (candidate != num && IsLocked(candidate) && IsDescendantOf(candidate, num))
                {
                    found.Add(candidate);
                }
            }

            return [.. found];
        }

        private bool IsDescendantOf(int candidate, int target)
        {
            for (var ancestor = ParentOf(candidate); ancestor >= 0; ancestor = ParentOf(ancestor))
            {
                if (ancestor == target)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private sealed class SubtreeDepthFirstSearchLockingTree : LockingTree
    {
        private readonly RootedTreeNode[] _nodes;

        public SubtreeDepthFirstSearchLockingTree(int[] parent)
            : base(parent) => _nodes = ParentArrayTree.Build(parent);

        public override int[] LockedDescendantsOf(int num)
        {
            var subtree = DepthFirstSearch.Traverse(_nodes[num], node => node.Children);

            return
            [
                .. subtree
                    .Where(node => node.Id != num && IsLocked(node.Id))
                    .Select(node => node.Id)
                    .Order()
            ];
        }
    }
}
