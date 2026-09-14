using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.SequentiallyOrdinalRankTracker;

// LeetCode 2102. Sequentially Ordinal Rank Tracker: a stream of scenic locations
// where the i-th Get returns the i-th best one seen so far - best meaning highest
// score, ties broken by the lexicographically smaller name.
//
// This is a design problem - LeetCode's own shape is a stateful object with two
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution/LRUCacheSolution use for their own design problems.
//
// CreateByResortEveryGet is the textbook baseline: keep every location seen so far
// in a plain List and re-sort it on each query. CreateByTwoHeaps is the composed
// answer, built entirely from this repo's own Heap<Element, TOrder>: the "kth
// largest with k growing by one every query" trick, whose whole point is that no
// sort ever happens.
//
// Both rank locations by the key (-score, name), so ValueTuple's own lexicographic
// IComparable already encodes "higher score first, ties broken by ascending name" -
// no custom IHeapOrder witness is needed beyond the two the library already ships.
internal static class SequentiallyOrdinalRankTrackerSolution
{
    public static IRankTracker CreateByResortEveryGet() => new ResortEveryGetRankTracker();

    public static IRankTracker CreateByTwoHeaps() => new TwoHeapRankTracker();

    // LeetCode's own SORTracker surface: add a location, then ask for the next
    // ordinal rank. Both strategies answer it, so a harness can replay one call
    // script against either of them.
    internal interface IRankTracker
    {
        void Add(string name, int score);

        string Get();
    }

    // The textbook answer: hold every location seen so far and re-sort on every
    // query, then index straight to the query count. Deliberately written without
    // this repo's primitives - it is the arm the two-heap tracker below has to
    // justify itself against, and until this migration it existed only as an
    // unasserted benchmark baseline.
    private sealed class ResortEveryGetRankTracker : IRankTracker
    {
        private readonly List<(int Score, string Name)> _seen = [];
        private int _queries;

        public void Add(string name, int score) => _seen.Add((score, name));

        public string Get()
        {
            _seen.Sort(static (a, b) =>
                a.Score != b.Score ? b.Score.CompareTo(a.Score) : string.CompareOrdinal(a.Name, b.Name));

            return _seen[_queries++].Name;
        }
    }

    // topK is a max-heap over (-score, name) holding the current best locations,
    // so the WORST of the best sits at its root; backup is a min-heap over the same
    // key holding every leftover candidate, so the BEST of the rest sits at its
    // root. Add always nets to "push into topK, then demote its new worst back into
    // backup," which pins topK's size at however many times Get has been called;
    // Get promotes backup's best into topK and returns the name now sitting at
    // topK's root - exactly the count-th best location.
    private sealed class TwoHeapRankTracker : IRankTracker
    {
        private readonly Heap<(int NegatedScore, string Name), MaxHeapOrder<(int, string)>> _topK = new();
        private readonly Heap<(int NegatedScore, string Name), MinHeapOrder<(int, string)>> _backup = new();

        public void Add(string name, int score)
        {
            _topK.Push((-score, name));
            _topK.TryPop(out var demoted);
            _backup.Push(demoted);
        }

        public string Get()
        {
            _backup.TryPop(out var promoted);
            _topK.Push(promoted);
            _topK.TryPeek(out var worstOfTop);

            return worstOfTop.Name;
        }
    }
}
