using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OperationsOnTree;

// LeetCode 1993. Operations on Tree: lock/unlock/upgrade over a parent-array tree. Ancestor checks
// (does locking node collide with a locked ancestor) are a plain walk up each node's own Parent
// field - Graph-domain topology contracts are deliberately child-ward only (GetChildren, never
// GetParent, see KthAncestorOfATreeNodeTests), so there is no witness for that direction to compose
// in the first place. Upgrade's "unlock every locked descendant" instead reuses this repo's own
// DepthFirstSearch.Traverse (Algorithms.Traversal.DepthFirst) - the bare-successor-function engine
// ARCHITECTURE.md §12 documents - called fresh per query with successors = node => node.Children to
// collect that node's whole subtree.
public sealed partial class OperationsOnTreeTests
{
    [Fact]
    public void LockingTree_LeetCodeExampleSequence_MatchesExpectedResults()
    {
        // node:      0
        //          / | \
        //         1  2  3
        //           / \
        //          4   5
        int[] parent = [-1, 0, 0, 0, 2, 2];
        var tree = new LockingTree(parent);

        (bool Expected, Func<bool> Operation)[] steps =
        [
            (true, () => tree.Lock(2, 2)),
            (false, () => tree.Unlock(2, 3)),
            (true, () => tree.Unlock(2, 2)),
            (true, () => tree.Lock(4, 5)),
            (true, () => tree.Upgrade(0, 1)),
            (false, () => tree.Lock(0, 1)),
        ];

        AssertOperationSequence(steps);
    }

    [Fact]
    public void Lock_AlreadyLockedNode_Fails()
    {
        int[] parent = [-1, 0];
        var tree = new LockingTree(parent);

        AssertOperationResult(true, () => tree.Lock(1, 7));

        AssertOperationResult(false, () => tree.Lock(1, 9));
    }

    [Fact]
    public void Upgrade_NodeItselfLocked_Fails()
    {
        int[] parent = [-1, 0, 0];
        var tree = new LockingTree(parent);
        tree.Lock(0, 1);
        tree.Lock(1, 2);

        AssertOperationResult(false, () => tree.Upgrade(0, 3));
    }

    [Fact]
    public void Upgrade_LockedAncestorExists_Fails()
    {
        // 0 -> 1 -> 2
        int[] parent = [-1, 0, 1];
        var tree = new LockingTree(parent);
        tree.Lock(0, 1);
        tree.Lock(2, 2);

        AssertOperationResult(false, () => tree.Upgrade(1, 3));
    }

    [Fact]
    public void Upgrade_NoLockedDescendant_Fails()
    {
        int[] parent = [-1, 0, 0];
        var tree = new LockingTree(parent);

        AssertOperationResult(false, () => tree.Upgrade(0, 1));
    }

    // Shared shape behind every Lock/Unlock/Upgrade assertion above: run the
    // operation, name its result, then compare - encapsulated once instead of
    // repeating the same Assert(tree.Op(...)) shape at each call site.
    private static void AssertOperationResult(bool expected, Func<bool> operation)
    {
        var actual = operation();
        Assert.Equal(expected, actual);
    }

    private static void AssertOperationSequence(IEnumerable<(bool Expected, Func<bool> Operation)> steps)
    {
        foreach (var (expected, operation) in steps)
        {
            AssertOperationResult(expected, operation);
        }
    }

    private sealed class OperationsOnTreeNode(int id)
    {
        public int Id { get; } = id;

        public List<OperationsOnTreeNode> Children { get; } = [];

        public OperationsOnTreeNode? Parent { get; set; }

        public int LockedBy { get; set; }
    }

    private sealed class LockingTree
    {
        private readonly OperationsOnTreeNode[] _nodes;

        public LockingTree(int[] parent)
        {
            _nodes = new OperationsOnTreeNode[parent.Length];

            for (var i = 0; i < parent.Length; i++)
            {
                _nodes[i] = new OperationsOnTreeNode(i);
            }

            for (var i = 1; i < parent.Length; i++)
            {
                _nodes[i].Parent = _nodes[parent[i]];
                _nodes[parent[i]].Children.Add(_nodes[i]);
            }
        }

        public bool Lock(int num, int user)
        {
            var node = _nodes[num];

            if (node.LockedBy != 0)
            {
                return false;
            }

            node.LockedBy = user;
            return true;
        }

        public bool Unlock(int num, int user)
        {
            var node = _nodes[num];

            if (node.LockedBy != user)
            {
                return false;
            }

            node.LockedBy = 0;
            return true;
        }

        public bool Upgrade(int num, int user)
        {
            var node = _nodes[num];

            if (node.LockedBy != 0 || HasLockedAncestor(node))
            {
                return false;
            }

            var lockedDescendants = FindLockedDescendants(node);

            if (lockedDescendants.Count == 0)
            {
                return false;
            }

            foreach (var descendant in lockedDescendants)
            {
                descendant.LockedBy = 0;
            }

            node.LockedBy = user;
            return true;
        }

        private static bool HasLockedAncestor(OperationsOnTreeNode node)
        {
            for (var ancestor = node.Parent; ancestor is not null; ancestor = ancestor.Parent)
            {
                if (ancestor.LockedBy != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<OperationsOnTreeNode> FindLockedDescendants(OperationsOnTreeNode node)
        {
            var subtree = DepthFirstSearch.Traverse(node, n => n.Children);
            return subtree.Where(n => n != node && n.LockedBy != 0).ToList();
        }
    }
}
