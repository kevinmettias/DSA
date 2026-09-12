using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.DesignCircularQueue;

// LeetCode 622. Design Circular Queue: a fixed-capacity FIFO with O(1) Front/Rear
// peeks at both ends of the same wraparound buffer.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and six operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared ICircularQueue surface below, instead of two static
// methods sharing an <Operation>By<Strategy> name, the same shape
// DesignTaskManagerSolution uses for its own instance-API problem (LC 3408).
internal static class DesignCircularQueueSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ICircularQueue
    {
        bool EnQueue(int value);

        bool DeQueue();

        int Front();

        int Rear();

        bool IsEmpty();

        bool IsFull();
    }

    // The textbook baseline this composition has to justify itself against: a
    // fixed BCL int[] with head/count tracking the wraparound by hand -
    // deliberately without this repo's Deque<int>, the arm the composed strategy
    // below has to justify itself against.
    internal sealed class CircularQueueByArrayBacked : ICircularQueue
    {
        private readonly int[] _items;
        private int _head;
        private int _count;

        public CircularQueueByArrayBacked(int capacity) => _items = new int[capacity];

        public bool IsEmpty() => _count == 0;

        public bool IsFull() => _count == _items.Length;

        public bool EnQueue(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _items[(_head + _count) % _items.Length] = value;
            _count++;
            return true;
        }

        public bool DeQueue()
        {
            if (IsEmpty())
            {
                return false;
            }

            _head = (_head + 1) % _items.Length;
            _count--;
            return true;
        }

        public int Front() => IsEmpty() ? LeetCodeAnswer.None : _items[_head];

        public int Rear() => IsEmpty() ? LeetCodeAnswer.None : _items[(_head + _count - 1) % _items.Length];
    }

    // The composed answer: this repo's own Deque<int> (already a wraparound-array
    // Representation, ARCHITECTURE.md §4.1's "earned by needing O(1) at both
    // ends") with an external capacity bound layered on top for IsFull - the same
    // "compose, don't invent a new representation" move MinStackSolution makes
    // over Stack<int>.
    internal sealed class CircularQueueByDequeBacked : ICircularQueue
    {
        private readonly RepoDeque _items = new();
        private readonly int _capacity;

        public CircularQueueByDequeBacked(int capacity) => _capacity = capacity;

        public bool IsEmpty() => _items.Count == 0;

        public bool IsFull() => _items.Count == _capacity;

        public bool EnQueue(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _items.PushBack(value);
            return true;
        }

        public bool DeQueue()
        {
            if (IsEmpty())
            {
                return false;
            }

            _items.TryPopFront(out _);
            return true;
        }

        public int Front() => _items.TryPeekFront(out var value) ? value : LeetCodeAnswer.None;

        public int Rear() => _items.TryPeekBack(out var value) ? value : LeetCodeAnswer.None;
    }
}
