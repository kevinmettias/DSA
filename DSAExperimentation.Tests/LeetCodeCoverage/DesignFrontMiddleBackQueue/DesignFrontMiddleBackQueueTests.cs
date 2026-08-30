using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignFrontMiddleBackQueue;

// LeetCode 1670. Design Front Middle Back Queue: the classic two-deque split - a front
// half and a back half kept within one element of each other in size - built directly
// over this repo's own Deque<int> (wraparound-array Representation, ARCHITECTURE.md
// §4.1), the same "compose, don't invent a new representation" move
// DesignCircularDequeTests already makes over the same Deque<int>.
public sealed partial class DesignFrontMiddleBackQueueTests
{
    [Fact]
    public void Operations_LeetCodeExampleSequence_MatchesExpectedResults()
    {
        var queue = new FrontMiddleBackQueue();

        queue.PushFront(1);   // [1]
        queue.PushBack(2);    // [1, 2]
        queue.PushMiddle(3);  // [1, 3, 2]
        queue.PushMiddle(4);  // [1, 4, 3, 2]

        Assert.Equal(1, queue.PopFront());  // [4, 3, 2]
        Assert.Equal(3, queue.PopMiddle()); // [4, 2]
        Assert.Equal(4, queue.PopMiddle()); // [2]
        Assert.Equal(2, queue.PopBack());   // []
        Assert.Equal(-1, queue.PopFront());
    }

    [Fact]
    public void PopMiddle_EvenElementCount_FavorsTheLeftOfTheTwoMiddleElements()
    {
        var queue = new FrontMiddleBackQueue();
        queue.PushBack(1);
        queue.PushBack(2);
        queue.PushBack(3);
        queue.PushBack(4);

        Assert.Equal(2, queue.PopMiddle());
        Assert.Equal(3, queue.PopMiddle());
    }

    private sealed class FrontMiddleBackQueue
    {
        private readonly RepoDeque _front = new();
        private readonly RepoDeque _back = new();

        public void PushFront(int value)
        {
            _front.PushFront(value);
            Rebalance();
        }

        public void PushMiddle(int value)
        {
            if (_front.Count < _back.Count)
            {
                _front.PushBack(value);
            }
            else
            {
                _back.PushFront(value);
            }
        }

        public void PushBack(int value)
        {
            _back.PushBack(value);
            Rebalance();
        }

        public int PopFront()
        {
            if (_front.Count == 0 && _back.Count == 0)
            {
                return -1;
            }

            int value;
            if (_front.Count > 0)
            {
                _front.TryPopFront(out value);
            }
            else
            {
                _back.TryPopFront(out value);
            }

            Rebalance();
            return value;
        }

        public int PopMiddle()
        {
            if (_front.Count == 0 && _back.Count == 0)
            {
                return -1;
            }

            if (_front.Count == _back.Count)
            {
                _front.TryPopBack(out var value);
                return value;
            }

            _back.TryPopFront(out var back);
            return back;
        }

        public int PopBack()
        {
            if (_front.Count == 0 && _back.Count == 0)
            {
                return -1;
            }

            int value;
            if (_back.Count > 0)
            {
                _back.TryPopBack(out value);
            }
            else
            {
                _front.TryPopBack(out value);
            }

            Rebalance();
            return value;
        }

        private void Rebalance()
        {
            if (_front.Count > _back.Count + 1)
            {
                _front.TryPopBack(out var value);
                _back.PushFront(value);
            }
            else if (_back.Count > _front.Count + 1)
            {
                _back.TryPopFront(out var value);
                _front.PushBack(value);
            }
        }
    }
}
