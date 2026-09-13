using RepoDynamicArray = DSAExperimentation.DataStructures.DynamicArray.DynamicArray<string>;

namespace DSAExperimentation.LeetCode.DesignBrowserHistory;

// LeetCode 1472. Design Browser History: a cursor into a growable sequence that
// truncates everything past the cursor on a new Visit, with Back/Forward clamped
// to the ends of what survives.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and three operations, not a single return value - so "every strategy
// for the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared IBrowserHistory surface below, the same shape
// DesignCircularQueueSolution uses for its own instance-API problem (LC 622).
internal static class DesignBrowserHistorySolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IBrowserHistory
    {
        void Visit(string url);

        string Back(int steps);

        string Forward(int steps);
    }

    // The textbook baseline this composition has to justify itself against: a BCL
    // List<string> plus a cursor, using RemoveRange to drop the discarded forward
    // history in one call - deliberately without this repo's DynamicArray<string>.
    internal sealed class BrowserHistoryByListBacked : IBrowserHistory
    {
        private readonly List<string> _history;
        private int _current;

        public BrowserHistoryByListBacked(string homepage) => _history = [homepage];

        public void Visit(string url)
        {
            _history.RemoveRange(_current + 1, _history.Count - _current - 1);
            _history.Add(url);
            _current++;
        }

        public string Back(int steps)
        {
            _current = Math.Max(0, _current - steps);
            return _history[_current];
        }

        public string Forward(int steps)
        {
            _current = Math.Min(_history.Count - 1, _current + steps);
            return _history[_current];
        }
    }

    // The composed answer: this repo's own DynamicArray<string> doing the same
    // truncate-then-append, the "sequence + access constraint" composition
    // Stack<T> already makes over DynamicArray<T> (ARCHITECTURE.md §4.1), just
    // with truncate-on-write layered on instead of LIFO. DynamicArray exposes no
    // bulk RemoveRange, so the discarded tail is popped one element at a time.
    internal sealed class BrowserHistoryByDynamicArrayBacked : IBrowserHistory
    {
        private readonly RepoDynamicArray _history = new();
        private int _current;

        public BrowserHistoryByDynamicArrayBacked(string homepage) => _history.Add(homepage);

        public void Visit(string url)
        {
            while (_history.Count > _current + 1)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            _history.Add(url);
            _current++;
        }

        public string Back(int steps)
        {
            _current = Math.Max(0, _current - steps);
            return _history.Get(_current);
        }

        public string Forward(int steps)
        {
            _current = Math.Min(_history.Count - 1, _current + steps);
            return _history.Get(_current);
        }
    }
}
