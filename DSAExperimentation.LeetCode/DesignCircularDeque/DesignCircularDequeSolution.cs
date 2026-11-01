using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.LeetCode.DesignCircularDeque;

// LeetCode 641. Design Circular Deque: insert/delete at both ends of a fixed-
// capacity buffer.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and eight operations, not a single return value - so "every
// strategy for the problem" (ARCHITECTURE.md §17.3) takes the form of two full
// classes implementing the shared ICircularDeque surface below, the same shape
// DesignCircularQueueSolution uses for its own instance-API problem (LC 622).
internal static class DesignCircularDequeSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ICircularDeque
    {
        bool InsertFront(int value);

        bool InsertLast(int value);

        bool DeleteFront();

        bool DeleteLast();

        int GetFront();

        int GetRear();

        bool IsEmpty();

        bool IsFull();
    }

    // The textbook baseline this composition has to justify itself against: a
    // fixed BCL int[] with head/count tracking the wraparound by hand at both
    // ends - deliberately without this repo's BoundedDeque<int>, the arm the composed
    // strategy below has to justify itself against.
    internal sealed class CircularDequeByArrayBacked : ICircularDeque
    {
        private readonly int[] _items;
        private int _head;
        private int _count;

        public CircularDequeByArrayBacked(int capacity) => _items = new int[capacity];

        public bool IsEmpty() => _count == 0;

        public bool IsFull() => _count == _items.Length;

        public bool InsertFront(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _head = (_head - 1 + _items.Length) % _items.Length;
            _items[_head] = value;
            _count++;
            return true;
        }

        public bool InsertLast(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _items[(_head + _count) % _items.Length] = value;
            _count++;
            return true;
        }

        public bool DeleteFront()
        {
            if (IsEmpty())
            {
                return false;
            }

            _head = (_head + 1) % _items.Length;
            _count--;
            return true;
        }

        public bool DeleteLast()
        {
            if (IsEmpty())
            {
                return false;
            }

            _count--;
            return true;
        }

        public int GetFront() => IsEmpty() ? LeetCodeAnswer.None : FrontItem();

        // The item at the head slot, which the empty guard above has made safe to read.
        private int FrontItem() => _items[_head];

        public int GetRear() =>
            IsEmpty() ? LeetCodeAnswer.None : RearItem();

        // The item at the rear slot: one step back from the head, wrapped by the buffer
        // length so the read stays inside the array.
        private int RearItem() => _items[(_head + _count - 1 + _items.Length) % _items.Length];
    }

    // The composed answer: this repo's own BoundedDeque<int>, which is this problem
    // exactly - both ends of the wraparound buffer, with the capacity an injected
    // invariant that refuses a push when full (ARCHITECTURE.md §20). Every operation
    // forwards, and only LeetCode's -1 for an empty peek is the problem's.
    internal sealed class CircularDequeByBoundedDeque(int capacity) : ICircularDeque
    {
        private readonly BoundedDeque<int> _items = new(capacity);

        public bool IsEmpty() => _items.Count == 0;

        public bool IsFull() => _items.IsFull;

        public bool InsertFront(int value) => _items.TryPushFront(value);

        public bool InsertLast(int value) => _items.TryPushBack(value);

        public bool DeleteFront() => _items.TryPopFront(out _);

        public bool DeleteLast() => _items.TryPopBack(out _);

        public int GetFront() => _items.TryPeekFront(out var value) ? value : LeetCodeAnswer.None;

        public int GetRear() => _items.TryPeekBack(out var value) ? value : LeetCodeAnswer.None;
    }
}
