using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

// LeetCode 232. Implement Queue using Stacks: an instance API (push/pop/peek,
// plus the emptiness check LeetCode spells `empty`) built from two of this
// repo's own Stack<int> instances. Elements
// always land on the "in" stack; a pop/peek transfers everything to the "out"
// stack only when that one is empty, so each element crosses at most twice
// across its whole lifetime - amortized O(1) pop/peek despite any single
// transfer being O(n).
//
// A design problem's whole point is which stack maneuver you pick, so unlike
// a plain function there is no separate "textbook baseline" arm to reconcile
// here: the original benchmark's Baseline/PrimitiveComposed methods were both
// `return 1` compile-smoke placeholders (see the manifest note on id 232),
// not a second algorithm, so this is the only strategy - the same situation
// BinarySearchTreeIteratorSolution documents for LC 173.
internal static class ImplementQueueUsingStacksSolution
{
    public static TwoStackQueue CreateByTwoStacks() => new();

    internal sealed class TwoStackQueue
    {
        private readonly RepoStack _in = new();
        private readonly RepoStack _out = new();

        public void Push(int value) => _in.Push(value);

        public int Pop()
        {
            MoveIfOutIsEmpty();
            _out.TryPop(out var value);
            return value;
        }

        public int Peek()
        {
            MoveIfOutIsEmpty();
            _out.TryPeek(out var value);
            return value;
        }

        // Named IsEmpty, not LeetCode's own `empty`: TwoStackQueue is this repo's
        // internal helper rather than the class LC 232 asks you to submit, it
        // implements no interface that would pin the spelling, and the sibling
        // design problems here (DesignCircularDeque, DesignCircularQueue) already
        // spell the same predicate IsEmpty.
        public bool IsEmpty() => _in.Count == 0 && _out.Count == 0;

        private void MoveIfOutIsEmpty()
        {
            if (_out.Count > 0)
            {
                return;
            }

            while (_in.TryPop(out var value))
            {
                _out.Push(value);
            }
        }
    }
}
