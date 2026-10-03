using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.RecoverBinarySearchTree;

// LeetCode 99. Recover Binary Search Tree: exactly two nodes had their values
// swapped by mistake. An in-order walk of a correct BST is non-decreasing, so the
// two violation points (prev.Value > node.Value) pin down the misplaced pair.
//
// LeetCode's actual operation is void and mutates the tree in place - the original
// benchmark arms only located the pair without writing the fix back, which was
// weaker than what the test's helper already proved correct, so both strategies
// here are promoted to perform the real recovery.
internal static class RecoverBinarySearchTreeSolution
{
    // The textbook baseline: collect the in-order sequence into a BCL List, then
    // scan it for the (at most two) descending steps. Written without this repo's
    // traversal engine - the arm InOrderHooks below has to justify itself against.
    public static void RecoverByManualRecursiveScan(BinaryTreeNode<int> root)
    {
        var values = new List<BinaryTreeNode<int>>();
        CollectInOrder(root, values);

        BinaryTreeNode<int>? first = null;
        BinaryTreeNode<int>? second = null;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i - 1].Value > values[i].Value)
            {
                first ??= values[i - 1];
                second = values[i];
            }
        }

        (first!.Value, second!.Value) = (second!.Value, first!.Value);
    }

    // This repo's own InOrderTraversal/IInOrderHooks walking the tree with no
    // intermediate list allocation; the two candidate nodes live in the hook, which the
    // walk hands back.
    public static void RecoverByInOrderHooks(BinaryTreeNode<int> root)
    {
        var scan = InOrderTraversal.Walk(root, new ScanHooks());

        var first = scan.First!;
        var second = scan.Second!;
        (first.Value, second.Value) = (second.Value, first.Value);
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<BinaryTreeNode<int>> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node);
        CollectInOrder(node.Right, values);
    }

    // Remembers the previous node and the first and last inversions it meets. Mutable by
    // design: the walk hands back the value it finished with.
    private struct ScanHooks : IInOrderHooks<int>
    {
        private BinaryTreeNode<int>? _previous;

        public BinaryTreeNode<int>? First { get; private set; }

        public BinaryTreeNode<int>? Second { get; private set; }

        public void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (_previous is { } previous && previous.Value > node.Value)
            {
                First ??= previous;
                Second = node;
            }

            _previous = node;
        }
    }
}
