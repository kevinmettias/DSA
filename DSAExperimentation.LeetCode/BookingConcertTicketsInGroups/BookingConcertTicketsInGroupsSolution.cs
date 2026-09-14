using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.BookingConcertTicketsInGroups;

// LeetCode 2286. Booking Concert Tickets in Groups: an instance API (gather/scatter)
// rather than a pure function, so "every strategy for the problem" (§17.3) takes the
// form of two full classes implementing the shared IBookMyShowStrategy surface below
// instead of two static methods sharing an <Operation>By<Strategy> name - a Design
// problem's whole point is a sequence of mutating calls against one instance, so
// there is no separate "prepare input" step to hoist into a benchmark's
// [GlobalSetup] the way OpenTheLock hoists a built graph; each [Benchmark] arm
// constructs its own instance and replays the same call script instead (the
// DesignTaskManagerSolution precedent).
//
// Both strategies track the same state - how many seats remain unbooked in each row -
// and differ only in what they can ask of it: a raw array has to scan, a pair of
// segment trees can answer "is there a qualifying row in this prefix" and "how many
// seats remain in this prefix" in logarithmic time.
internal static class BookingConcertTicketsInGroupsSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it. Gather returns LeetCode's own [row, seat] pair (empty when no
    // single row can seat the group); Scatter reports whether the group fit at all.
    internal interface IBookMyShowStrategy
    {
        int[] Gather(int k, int maxRow);

        bool Scatter(int k, int maxRow);
    }

    // The textbook answer: one int[] of remaining seats per row, scanned linearly -
    // Gather walks rows 0..maxRow looking for the first that can seat the whole
    // group, Scatter totals the same window and then fills it row by row. O(n) per
    // call, deliberately written without this repo's primitives; it is the arm the
    // composed strategy below has to justify itself against.
    internal sealed class BookMyShowByRowScan : IBookMyShowStrategy
    {
        private readonly int _seatsPerRow;
        private readonly int[] _available;

        public BookMyShowByRowScan(int rowCount, int seatsPerRow)
        {
            _seatsPerRow = seatsPerRow;
            _available = new int[rowCount];
            Array.Fill(_available, seatsPerRow);
        }

        public int[] Gather(int k, int maxRow)
        {
            for (var row = 0; row <= maxRow; row++)
            {
                if (_available[row] >= k)
                {
                    var seat = _seatsPerRow - _available[row];
                    _available[row] -= k;

                    return [row, seat];
                }
            }

            return [];
        }

        public bool Scatter(int k, int maxRow)
        {
            if (RemainingSeats(maxRow) < k)
            {
                return false;
            }

            for (var row = 0; row <= maxRow && k > 0; row++)
            {
                var take = Math.Min(_available[row], k);
                _available[row] -= take;
                k -= take;
            }

            return true;
        }

        private long RemainingSeats(int maxRow)
        {
            var total = 0L;

            for (var row = 0; row <= maxRow; row++)
            {
                total += _available[row];
            }

            return total;
        }
    }

    // This repo's own SegmentTree over the same per-row remaining-seat values, once
    // under MaxOperation<int> and once under SumOperation<int>. The max tree turns
    // "the leftmost row in [fromRow, maxRow] with at least threshold free seats"
    // into a binary search on the answer: a prefix max over a growing window is
    // monotonically non-decreasing, so HasCapacitySequence below is
    // [false...false, true...true] and BinarySearch.LowerBound finds its boundary -
    // the same search-on-the-answer idiom KokoEatingBananasSolution uses, just with
    // the monotone answer being a row index rather than a numeric parameter. The sum
    // tree answers Scatter's feasibility check with one range query instead of a
    // per-row total. Each booking is one point Update on each tree.
    internal sealed class BookMyShowBySegmentTreeBinarySearch : IBookMyShowStrategy
    {
        private const int OneSeat = 1;

        private readonly int _seatsPerRow;
        private readonly SegmentTree<int, MaxOperation<int>> _maxAvailable;
        private readonly SegmentTree<int, SumOperation<int>> _sumAvailable;

        public BookMyShowBySegmentTreeBinarySearch(int rowCount, int seatsPerRow)
        {
            _seatsPerRow = seatsPerRow;
            var initial = new int[rowCount];
            Array.Fill(initial, seatsPerRow);
            _maxAvailable = new SegmentTree<int, MaxOperation<int>>(initial);
            _sumAvailable = new SegmentTree<int, SumOperation<int>>(initial);
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
            row = FindLeftmostRowWithCapacity(row, maxRow, threshold: OneSeat)!.Value;

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

        // Get(index) is "some row in [fromRow, fromRow + index] still has at least
        // threshold free seats". A prefix max over a growing window only ever rises,
        // so this is false up to the qualifying row and true from there on - the
        // monotonicity BinarySearch.LowerBound assumes but never checks. A witness
        // for this problem alone: the predicate is BookMyShow's own seating rule,
        // not a general monotone-predicate shape.
        private readonly struct HasCapacitySequence(
            SegmentTree<int, MaxOperation<int>> availableSeats, int fromRow, int maxRow, int threshold)
            : IRandomAccessSequence<bool>
        {
            public int Length => maxRow - fromRow + 1;

            public bool Get(int index) => availableSeats.Query(fromRow, fromRow + index) >= threshold;
        }
    }
}
