using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignCircularDeque;

// LeetCode 641. Design Circular Deque: insert/delete at both ends of a fixed-
// capacity buffer is exactly this repo's Deque<int> (wraparound-array
// Representation, ARCHITECTURE.md §4.1) with an external capacity bound layered on
// top for IsFull - the same "compose, don't invent a new representation" move
// DesignCircularQueueTests already makes over the same Deque<int>, just exercising
// both ends instead of one.
public sealed partial class DesignCircularDequeTests
{
    [Fact]
    public void InsertDeleteBothEnds_LeetCodeExample_MatchesExpectedSequence()
    {
        var deque = new MyCircularDeque(3);

        Assert.True(deque.InsertLast(1));
        Assert.True(deque.InsertLast(2));
        Assert.True(deque.InsertFront(3));
        Assert.False(deque.InsertFront(4));
        Assert.Equal(2, deque.GetRear());
        Assert.True(deque.IsFull());
        Assert.True(deque.DeleteLast());
        Assert.True(deque.InsertFront(4));
        Assert.Equal(4, deque.GetFront());
    }

    [Fact]
    public void DeleteFrontAndDeleteLast_EmptyDeque_ReturnFalseAndPeeksReturnNegativeOne()
    {
        var deque = new MyCircularDeque(1);

        Assert.True(deque.IsEmpty());
        Assert.False(deque.DeleteFront());
        Assert.False(deque.DeleteLast());
        Assert.Equal(-1, deque.GetFront());
        Assert.Equal(-1, deque.GetRear());
    }

    private sealed class MyCircularDeque
    {
        private readonly RepoDeque _items = new();
        private readonly int _capacity;

        public MyCircularDeque(int k) => _capacity = k;

        public bool InsertFront(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _items.PushFront(value);
            return true;
        }

        public bool InsertLast(int value)
        {
            if (IsFull())
            {
                return false;
            }

            _items.PushBack(value);
            return true;
        }

        public bool DeleteFront()
        {
            if (IsEmpty())
            {
                return false;
            }

            _items.TryPopFront(out _);
            return true;
        }

        public bool DeleteLast()
        {
            if (IsEmpty())
            {
                return false;
            }

            _items.TryPopBack(out _);
            return true;
        }

        public int GetFront() => _items.TryPeekFront(out var value) ? value : -1;

        public int GetRear() => _items.TryPeekBack(out var value) ? value : -1;

        public bool IsEmpty() => _items.Count == 0;

        public bool IsFull() => _items.Count == _capacity;
    }
}
