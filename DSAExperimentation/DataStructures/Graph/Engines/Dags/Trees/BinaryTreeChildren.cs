using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Computed on demand from Left/Right, not stored as a list - the same on-demand
// shape GridChildren uses for geometry, applied here to two named slots instead of
// four directional offsets. Count/Get compact away null slots (Left-then-Right
// order) so the generic n-ary walkers (DepthFirstTraversal, LevelGroupedBreadthFirst
// Traversal, TreeMetrics, LowestCommonAncestor, AllRootToLeafPaths) never have to
// special-case a missing child - they just see whatever children exist.
//
// That compaction is exactly why InOrderTraversal (DataStructures/BinaryTree/
// InOrderTraversal.cs) does NOT close its generics over this type: compacting away a
// missing Left throws away positional identity. A node with Left=null, Right=X has
// Count=1 and Get(0)=X, indistinguishable from a node whose only child is a Left.
// A traversal that meant "recurse into child 0, visit, recurse into the rest" would
// walk X's whole subtree before visiting the node itself - backwards for every
// right-only node. In-order is fundamentally about slot identity ("before" vs.
// "after"), which this compacted view cannot preserve, so it stays hardwired to
// Left/Right directly instead.
internal readonly struct BinaryTreeChildren<TValue>(BinaryTreeNode<TValue> node) : IChildren<BinaryTreeNode<TValue>>
{
    public int Count
    {
        get
        {
            var count = 0;

            if (node.Left is not null)
            {
                count++;
            }

            if (node.Right is not null)
            {
                count++;
            }

            return count;
        }
    }

    public BinaryTreeNode<TValue> Get(int index)
    {
        if (node.Left is not null)
        {
            if (index == 0)
            {
                return node.Left;
            }

            index--;
        }

        if (node.Right is not null && index == 0)
        {
            return node.Right;
        }

        throw new IndexOutOfRangeException();
    }
}
