using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.FindConsecutiveIntegersFromADataStream;

// LeetCode 2526. Find Consecutive Integers from a Data Stream: a DataStream is
// constructed from a target `value` and a window size `k`, then asked, one streamed
// integer at a time, whether the last k integers to arrive are all equal to `value` -
// answering false until at least k integers have arrived at all.
//
// A Design problem's whole point is a sequence of calls against one instance, so
// "every strategy for the problem" (ARCHITECTURE.md section 17.3) takes the form of
// two classes implementing the shared IDataStreamStrategy surface below, the same
// shape StreamOfCharactersSolution uses for LC 1032.
internal static class FindConsecutiveIntegersFromADataStreamSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one stream against either strategy without restating it.
    internal interface IDataStreamStrategy
    {
        bool Consec(int num);
    }

    // The textbook answer: keep every arrival in a BCL List<int> that is never
    // trimmed, and rescan its last k entries on every call - O(k) per call, O(n * k)
    // over a stream of n arrivals. Deliberately written without this repo's
    // primitives - it is the arm the windowed strategy has to justify itself against.
    internal sealed class DataStreamByHistoryRescan(int value, int k) : IDataStreamStrategy
    {
        private readonly List<int> _history = [];

        public bool Consec(int num)
        {
            _history.Add(num);

            if (_history.Count < k)
            {
                return false;
            }

            for (var i = _history.Count - k; i < _history.Count; i++)
            {
                if (_history[i] != value)
                {
                    return false;
                }
            }

            return true;
        }
    }

    // This repo's own Deque<int> holding a fixed-size window of the last k arrivals -
    // PushBack on arrival, TryPopFront once the window grows past k - the same
    // windowing shape SlidingWindowMaximum runs over Deque<int>. The count of window
    // slots currently equal to `value` is maintained incrementally as items enter and
    // leave, so a call costs O(1) instead of rescanning the window: the whole stream
    // is O(n), and the window never grows past k entries.
    internal sealed class DataStreamByFixedWindow(int value, int k) : IDataStreamStrategy
    {
        private readonly RepoDeque _window = new();
        private int _matchCount;

        public bool Consec(int num)
        {
            _window.PushBack(num);

            if (num == value)
            {
                _matchCount++;
            }

            if (EvictsAMatchedValue(_window, k, value))
            {
                _matchCount--;
            }

            return _window.Count == k && _matchCount == k;
        }

        // The window has grown past k and the entry leaving its front is one of
        // the values being counted, so the running match count gives it back.
        private static bool EvictsAMatchedValue(RepoDeque window, int k, int value)
            => window.Count > k && window.TryPopFront(out var evicted) && evicted == value;
    }
}
