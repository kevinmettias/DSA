using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ImplementQueueUsingStacks;

// LeetCode 232. Implement Queue using Stacks: an instance API (push/pop/peek,
// plus the emptiness check LeetCode spells `empty`) built from two stacks.
// Elements always land on the "in" stack; a pop/peek transfers everything to the
// "out" stack only when that one is empty, so each element crosses at most twice
// across its whole lifetime - amortized O(1) pop/peek despite any single
// transfer being O(n).
//
// A design problem's whole point is which stack maneuver you pick, and both
// strategies are that one maneuver: on two of the BCL's Stack<int>, or on two of
// this repo's own, which is the comparison the benchmark makes.
internal static class ImplementQueueUsingStacksSolution
{
    // The textbook answer: the two-stack transfer over the BCL's Stack<int>, the arm this
    // repo's stack is measured against (ARCHITECTURE 17.5).
    public static BclTwoStackQueue CreateByTwoBclStacks() => new();

    public static TwoStackQueue CreateByTwoStacks() => new();

    // LeetCode's own MyQueue contract, which both queues keep. Its emptiness check is
    // IsEmpty rather than LeetCode's `empty`: these queues are this repo's helpers, not
    // the class LC 232 asks you to submit, and the sibling design problems here
    // (DesignCircularDeque, DesignCircularQueue) already spell the same predicate IsEmpty.
    internal interface IStackQueue
    {
        void Push(int value);

        int Pop();

        int Peek();

        bool IsEmpty();
    }

    internal sealed class BclTwoStackQueue : IStackQueue
    {
        private readonly Stack<int> _in = new();
        private readonly Stack<int> _out = new();

        public void Push(int value) => _in.Push(value);

        public int Pop()
        {
            MoveIfOutIsEmpty();
            return _out.Pop();
        }

        public int Peek()
        {
            MoveIfOutIsEmpty();
            return _out.Peek();
        }

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

    internal sealed class TwoStackQueue : IStackQueue
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
