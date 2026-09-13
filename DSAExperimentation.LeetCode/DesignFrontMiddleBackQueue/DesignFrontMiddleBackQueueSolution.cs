using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.DesignFrontMiddleBackQueue;

// LeetCode 1670. Design Front Middle Back Queue: push and pop at three positions -
// the front, the back, and the middle. LeetCode fixes what "middle" means on an
// even-length queue: a push lands at index n / 2, and a pop takes index
// (n - 1) / 2, so both favour the left of the two middle elements.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and six operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md section 17.3) takes the form of two full
// classes implementing the shared IFrontMiddleBackQueue surface below, the same
// shape DesignCircularDequeSolution uses for its own instance-API problem (LC 641).
internal static class DesignFrontMiddleBackQueueSolution
{
    // Both "middle" positions are a halving of the current length; the two
    // strategies below round differently on push and pop, not differently from
    // each other.
    private const int MiddleDivisor = 2;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it. Count is not one of LeetCode's six operations - it is here so
    // a harness can observe the queue without draining it, which is exactly what
    // both of this problem's original benchmark arms returned.
    internal interface IFrontMiddleBackQueue
    {
        int Count { get; }

        void PushFront(int value);

        void PushMiddle(int value);

        void PushBack(int value);

        int PopFront();

        int PopMiddle();

        int PopBack();
    }

    // The textbook baseline this composition has to justify itself against: one
    // BCL List<int>, with every position expressed as an index into it. Correct by
    // construction - the index arithmetic is LeetCode's own wording - but a front
    // or middle push shifts every following element, so each is O(n).
    internal sealed class FrontMiddleBackQueueByListInsert : IFrontMiddleBackQueue
    {
        private readonly List<int> _items = [];

        public int Count => _items.Count;

        public void PushFront(int value) => _items.Insert(0, value);

        public void PushMiddle(int value) => _items.Insert(_items.Count / MiddleDivisor, value);

        public void PushBack(int value) => _items.Add(value);

        public int PopFront() => RemoveAt(0);

        public int PopMiddle() => RemoveAt((_items.Count - 1) / MiddleDivisor);

        public int PopBack() => RemoveAt(_items.Count - 1);

        private int RemoveAt(int index)
        {
            if (_items.Count == 0)
            {
                return LeetCodeAnswer.None;
            }

            var value = _items[index];
            _items.RemoveAt(index);
            return value;
        }
    }

    // The composed answer: the classic two-deque split, built over this repo's own
    // Deque<int> (a wraparound-array Representation, ARCHITECTURE.md section 4.1),
    // the same "compose, don't invent a new representation" move
    // DesignCircularDequeSolution makes over the same Deque<int>. Every one of the
    // six operations is O(1) amortized, across the single-element rebalance move
    // between the two halves.
    //
    // The invariant Rebalance restores is the load-bearing part: the back half
    // carries the extra element, so _front.Count is either _back.Count or
    // _back.Count - 1 and never more. That is what makes "the middle" a deque end
    // rather than an index - with the front half allowed to run long instead, the
    // element at index n / 2 stops being either half's boundary element and both
    // middle operations address the wrong element.
    internal sealed class FrontMiddleBackQueueByTwoDeques : IFrontMiddleBackQueue
    {
        private readonly RepoDeque _front = new();
        private readonly RepoDeque _back = new();

        public int Count => _front.Count + _back.Count;

        public void PushFront(int value)
        {
            _front.PushFront(value);
            Rebalance();
        }

        // Lands at index n / 2 either way: appending to a short front half puts it
        // at _front.Count, and prepending to the back half when the halves are
        // even puts it at the same place. Needs no rebalance - both branches close
        // the gap rather than widening it.
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
            if (Count == 0)
            {
                return LeetCodeAnswer.None;
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

        // Index (n - 1) / 2 under the invariant: with equal halves that is the
        // front half's last element, and with the back half one longer it is the
        // back half's first. Both are deque ends, and both branches leave the
        // invariant intact, so no rebalance is needed.
        public int PopMiddle()
        {
            if (Count == 0)
            {
                return LeetCodeAnswer.None;
            }

            if (_front.Count == _back.Count)
            {
                _front.TryPopBack(out var middle);
                return middle;
            }

            _back.TryPopFront(out var value);
            return value;
        }

        public int PopBack()
        {
            if (Count == 0)
            {
                return LeetCodeAnswer.None;
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
            if (_front.Count > _back.Count)
            {
                _front.TryPopBack(out var overflow);
                _back.PushFront(overflow);
            }
            else if (_back.Count > _front.Count + 1)
            {
                _back.TryPopFront(out var underflow);
                _front.PushBack(underflow);
            }
        }
    }
}
