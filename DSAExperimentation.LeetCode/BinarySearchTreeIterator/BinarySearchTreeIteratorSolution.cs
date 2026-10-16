using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using NodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.LeetCode.BinarySearchTreeIterator;

// LeetCode 173. Binary Search Tree Iterator: HasNext()/Next() controls an in-order
// walk of a BST via an explicit left-spine stack, so next() runs in amortized O(1)
// time and O(h) memory, h = tree height - the shape LC 173's own follow-up asks
// for. The original inline test carried exactly this as a private helper; this is
// that same algorithm promoted to the solution tier, unchanged.
//
// Both strategies are that one walk and differ only in the stack the spine is kept
// on: the BCL's own Stack<T>, or this repo's.
internal static class BinarySearchTreeIteratorSolution
{
    // The textbook answer: the left-spine walk over the BCL's Stack<T>, the arm this
    // repo's stack is measured against (ARCHITECTURE 17.5).
    public static BclStackIterator CreateByBclStack(BinaryTreeNode<int>? root) => new(root);

    public static BstIterator CreateByLeftSpineStack(BinaryTreeNode<int>? root) => new(root);

    // LeetCode's own BSTIterator contract, which both iterators keep.
    internal interface IBstIterator
    {
        bool HasNext();

        int Next();
    }

    internal sealed class BclStackIterator : IBstIterator
    {
        private readonly Stack<BinaryTreeNode<int>> _stack = new();

        public BclStackIterator(BinaryTreeNode<int>? root) => PushLeftSpine(root);

        public bool HasNext() => _stack.Count > 0;

        public int Next()
        {
            var node = _stack.Pop();
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

    internal sealed class BstIterator : IBstIterator
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
