using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BookingConcertTicketsInGroups;

// LeetCode 2286. Booking Concert Tickets in Groups: two production SegmentTree
// instances over the same "seats still available per row" values - Max locates the
// leftmost qualifying row for Gather (HasCapacitySequence + BinarySearch.LowerBound,
// the same "binary search on the answer" idiom KokoEatingBananasTests already
// establishes, just with the monotone answer being a row index instead of a numeric
// parameter: a prefix max over a growing window is non-decreasing, so the window's
// running max can only cross the threshold at the row that itself supplies the
// qualifying value), Sum answers Scatter's feasibility check with one range query
// instead of a per-row scan.
public sealed partial class BookingConcertTicketsInGroupsTests
{
    [Fact]
    public void GatherAndScatter_LeetCodeExample_MatchesExpectedSequence()
    {
        var bookMyShow = new BookMyShow(2, 5);

        var firstGather = bookMyShow.Gather(4, 0);
        Assert.Equal([0, 0], firstGather);

        var secondGather = bookMyShow.Gather(2, 0);
        Assert.Equal([], secondGather);

        var firstScatter = bookMyShow.Scatter(5, 1);
        Assert.True(firstScatter);

        var secondScatter = bookMyShow.Scatter(5, 1);
        Assert.False(secondScatter);
    }

    [Fact]
    public void Gather_NoRowWithinMaxRowHasEnoughCapacity_ReturnsEmptyArray()
    {
        var bookMyShow = new BookMyShow(3, 3);

        var actual = bookMyShow.Gather(4, 2);
        Assert.Equal([], actual);
    }

    [Fact]
    public void Scatter_MoreSeatsRequestedThanRemainInRange_ReturnsFalse()
    {
        var bookMyShow = new BookMyShow(2, 3);

        var actual = bookMyShow.Scatter(7, 1);
        Assert.False(actual);
    }

    private sealed class BookMyShow
    {
        private readonly int _seatsPerRow;
        private readonly SegmentTree<int, MaxOperation<int>> _maxAvailable;
        private readonly SegmentTree<int, SumOperation<int>> _sumAvailable;

        public BookMyShow(int rowCount, int seatsPerRow)
        {
            _seatsPerRow = seatsPerRow;
            var initial = Enumerable.Repeat(seatsPerRow, rowCount).ToArray();
            _maxAvailable = new SegmentTree<int, MaxOperation<int>>(initial);
            _sumAvailable = new SegmentTree<int, SumOperation<int>>((int[])initial.Clone());
        }

        public int[] Gather(int k, int maxRow)
        {
            var row = FindLeftmostRowWithCapacity(fromRow: 0, maxRow, threshold: k);

            if (row is null)
            {
                return [];
            }

            var available = _maxAvailable.Query(row.Value, row.Value);
            var seat = _seatsPerRow - available;
            SetAvailable(row.Value, available - k);

            return [row.Value, seat];
        }

        public bool Scatter(int k, int maxRow)
        {
            if (_sumAvailable.Query(0, maxRow) < k)
            {
                return false;
            }

            var row = 0;

            while (k > 0)
            {
                (row, k) = ScatterStep(row, maxRow, k);
            }

            return true;
        }

        private (int Row, int K) ScatterStep(int row, int maxRow, int k)
        {
            row = FindLeftmostRowWithCapacity(row, maxRow, threshold: 1)!.Value;

            var available = _maxAvailable.Query(row, row);
            var take = Math.Min(available, k);
            SetAvailable(row, available - take);
            k -= take;

            if (take == available)
            {
                row++;
            }

            return (row, k);
        }

        private int? FindLeftmostRowWithCapacity(int fromRow, int maxRow, int threshold)
        {
            var sequence = new HasCapacitySequence(_maxAvailable, fromRow, maxRow, threshold);
            var offset = BinarySearch.LowerBound(sequence, true);

            return offset >= sequence.Length ? null : fromRow + offset;
        }

        private void SetAvailable(int row, int available)
        {
            _maxAvailable.Update(row, available);
            _sumAvailable.Update(row, available);
        }
    }

    // Whether a row with at least `threshold` available seats exists anywhere in the
    // growing window [fromRow, fromRow + index]. A prefix max over a growing window
    // is monotonically non-decreasing, so this is [false...false, true...true] over
    // index, exactly the shape KokoEatingBananasTests.FeasibleSpeedSequence already
    // uses for its own search-on-answer.
    private readonly struct HasCapacitySequence(
        SegmentTree<int, MaxOperation<int>> availableSeats, int fromRow, int maxRow, int threshold)
        : IRandomAccessSequence<bool>
    {
        public int Length => maxRow - fromRow + 1;

        public bool Get(int index) => availableSeats.Query(fromRow, fromRow + index) >= threshold;
    }
}
