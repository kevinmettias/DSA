using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.LeetCode.BinarySearchTreeIterator;

// LeetCode 173. Binary Search Tree Iterator: HasNext()/Next() controls an in-order
// walk of a BST via an explicit left-spine stack, so next() runs in amortized O(1)
// time and O(h) memory, h = tree height - the shape LC 173's own follow-up asks
// for. The original inline test carried exactly this as a private helper; this is
// that same algorithm promoted to the solution tier, unchanged.
internal static class BinarySearchTreeIteratorSolution
{
    public static BstIterator CreateByLeftSpineStack(BinaryTreeNode<int>? root) => new(root);

    internal sealed class BstIterator
    {
        private readonly NodeStack _stack = new();

        public BstIterator(BinaryTreeNode<int>? root) => PushLeftSpine(root);

        public bool HasNext() => _stack.Count > 0;

        public int Next()
        {
            _stack.TryPop(out var node);
            PushLeftSpine(node.Right);
            return node.Value;
        }

        private void PushLeftSpine(BinaryTreeNode<int>? node)
        {
            while (node is not null)
            {
                _stack.Push(node);
                node = node.Left;
            }
        }
    }
}
