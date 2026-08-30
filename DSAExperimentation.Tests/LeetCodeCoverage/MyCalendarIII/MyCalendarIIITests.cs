using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MyCalendarIII;

// LeetCode 732. My Calendar III: a delta/sweep-line count, using this repo's own
// HashMap<int,int> to accumulate a +1 at every booking's start and a -1 at its
// end (the standard technique for turning "how many intervals overlap right
// now" into a prefix-sum sweep over event points), then this repo's own
// MergeSort.Sort over an ArrayIndexedSequence<int> to walk those delta keys in
// ascending order every time Book is called - the same "no ordered-map
// primitive, so sort the keys instead of maintaining one" substitution
// RangeModuleTests already makes for a real TreeMap/NavigableMap. The running
// prefix sum's maximum after folding this booking's own +1/-1 in is exactly the
// "k" LeetCode wants back from Book.
public sealed partial class MyCalendarIIITests
{
    [Fact]
    public void Book_LeetCodeExample_ReturnsMaxOverlapAfterEachBooking()
    {
        var calendar = new MyCalendarThree();

        Assert.Equal(1, calendar.Book(10, 20));
        Assert.Equal(1, calendar.Book(50, 60));
        Assert.Equal(2, calendar.Book(10, 40));
        Assert.Equal(3, calendar.Book(5, 15));
        Assert.Equal(3, calendar.Book(5, 10));
        Assert.Equal(3, calendar.Book(25, 55));
    }

    [Fact]
    public void Book_NoOverlaps_AlwaysReturnsOne()
    {
        var calendar = new MyCalendarThree();

        Assert.Equal(1, calendar.Book(0, 5));
        Assert.Equal(1, calendar.Book(10, 15));
        Assert.Equal(1, calendar.Book(20, 25));
    }

    private sealed class MyCalendarThree
    {
        private readonly HashMap<int, int> _delta = new();

        public int Book(int start, int end)
        {
            AddDelta(start, 1);
            AddDelta(end, -1);

            var keys = _delta.Keys.ToArray();
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(keys));

            var running = 0;
            var maxOverlap = 0;

            foreach (var key in keys)
            {
                _delta.TryGetValue(key, out var change);
                running += change;
                maxOverlap = Math.Max(maxOverlap, running);
            }

            return maxOverlap;
        }

        private void AddDelta(int point, int amount)
        {
            var current = _delta.TryGetValue(point, out var existing) ? existing : 0;
            _delta.Set(point, current + amount);
        }
    }
}
