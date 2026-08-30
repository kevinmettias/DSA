using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularQueue;

// LeetCode 622. Design Circular Queue: a fixed-capacity FIFO with O(1) Front/Rear
// peeks at both ends of the same wraparound buffer - exactly this repo's Deque<int>
// (wraparound-array representation, ARCHITECTURE.md §4.1's "earned by needing O(1)
// at both ends") with an external capacity bound layered on top for IsFull, the
// same "compose, don't invent a new representation" move MinStackTests already makes
// over Stack<int>.
public sealed partial class DesignCircularQueueTests
{
    [Fact]
    public void EnQueueDeQueueFrontRear_LeetCodeExample_MatchesExpectedSequence()
    {
        var queue = new MyCircularQueue(3);

        Assert.True(queue.EnQueue(1));
        Assert.True(queue.EnQueue(2));
        Assert.True(queue.EnQueue(3));
        Assert.False(queue.EnQueue(4));
        Assert.Equal(3, queue.Rear());
        Assert.True(queue.IsFull());
        Assert.True(queue.DeQueue());
        Assert.True(queue.EnQueue(4));
        Assert.Equal(4, queue.Rear());
        Assert.Equal(2, queue.Front());
    }

    [Fact]
    public void DeQueue_EmptyQueue_ReturnsFalseAndStaysEmpty()
    {
        var queue = new MyCircularQueue(1);

        Assert.True(queue.IsEmpty());
        Assert.False(queue.DeQueue());
        Assert.True(queue.IsEmpty());
    }

    private sealed class MyCircularQueue
    {
        private readonly RepoDeque _items = new();
        private readonly int _capacity;

        public MyCircularQueue(int k) => _capacity = k;

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

        public int Front() => _items.TryPeekFront(out var value) ? value : -1;

        public int Rear() => _items.TryPeekBack(out var value) ? value : -1;

        public bool IsEmpty() => _items.Count == 0;

        public bool IsFull() => _items.Count == _capacity;
    }
}
