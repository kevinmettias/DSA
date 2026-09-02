using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SequentiallyOrdinalRankTracker;

// LeetCode 2102. Sequentially Ordinal Rank Tracker: the classic two-heap "streaming
// kth-largest with k growing by one every query" trick, built entirely from this
// repo's own Heap<T,TOrder>. Locations are keyed as (-score, name) so ValueTuple's
// own lexicographic IComparable already encodes "higher score first, ties broken by
// ascending name" - no custom IHeapOrder witness needed beyond the two the library
// already ships (MinHeapOrder/MaxHeapOrder).
public sealed partial class SequentiallyOrdinalRankTrackerTests
{
    [Fact]
    public void Get_OfficialExampleSequence_ReturnsSuccessiveRanks()
    {
        var tracker = new RankTracker();

        tracker.Add("bradford", 2);
        tracker.Add("branford", 3);
        Assert.Equal("branford", tracker.Get());

        tracker.Add("alps", 2);
        Assert.Equal("alps", tracker.Get());

        tracker.Add("orl", 2);
        Assert.Equal("bradford", tracker.Get());

        tracker.Add("orlando", 3);
        Assert.Equal("bradford", tracker.Get());

        tracker.Add("antibs", 2);
        Assert.Equal("bradford", tracker.Get());

        tracker.Add("forest", 4);
        Assert.Equal("bradford", tracker.Get());
    }

    [Fact]
    public void Get_StrictlyDescendingScores_ReturnsInInsertionOrder()
    {
        var tracker = new RankTracker();

        tracker.Add("first", 30);
        tracker.Add("second", 20);
        tracker.Add("third", 10);

        Assert.Equal("first", tracker.Get());
        Assert.Equal("second", tracker.Get());
        Assert.Equal("third", tracker.Get());
    }

    // topK is a size-`count` max-heap holding the current best `count` locations
    // (worst of the best sits at its root); backup is a min-heap holding every
    // leftover candidate (best of the rest sits at its root). Add always nets to
    // "push into topK, then demote its new worst back into backup," which pins
    // topK's size at however many times Get has been called; Get promotes
    // backup's best into topK and returns the name now sitting at topK's root -
    // exactly the count-th best location.
    private sealed class RankTracker
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
