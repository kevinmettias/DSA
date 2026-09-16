using DSAExperimentation.LeetCode.BookingConcertTicketsInGroups;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BookingConcertTicketsInGroups;

// Harness only. Both strategies are BookingConcertTicketsInGroupsSolution's - this
// file replays LeetCode's published call sequences against each
// IBookMyShowStrategy implementation via a small operation script, so a failure
// still names the strategy that broke even though the "input" here is a sequence of
// mutating calls rather than a single argument tuple - the same shape
// DesignTaskManagerTests already uses for its own instance-API problem. Unlike
// those scripts, gather and scatter answer with different types (a [row, seat] pair
// versus a bool), so each BookMyShowOp carries its own expected answer instead of
// the script being paired with one uniform expected[] array. BookMyShowOp is pure
// dispatch plus the assertion - no seating logic of its own.
//
// The pre-migration test only proved the segment-tree strategy; the row-scan
// baseline (previously untested scaffolding inlined in the benchmark, where its
// gather only ever reported whether a row was found rather than which row and seat)
// gets that same coverage here for the first time.
public sealed class BookingConcertTicketsInGroupsTests
{
    public static TheoryData<int, int, BookMyShowOp[]> Examples =>
        new()
        {
            {
                // LeetCode's own published example.
                2, 5,
                [
                    BookMyShowOp.Gather(4, 0, [0, 0]),
                    BookMyShowOp.Gather(2, 0, []),
                    BookMyShowOp.ScatterSeated(5, 1),
                    BookMyShowOp.ScatterNotSeated(5, 1),
                ]
            },
            {
                // No row within maxRow is wide enough to seat the group at all.
                3, 3, [BookMyShowOp.Gather(4, 2, [])]
            },
            {
                // Two rows of three seats is six seats; seven never fits.
                2, 3, [BookMyShowOp.ScatterNotSeated(7, 1)]
            },
            {
                // A single row consumed by successive gathers: the reported seat is
                // the first still-free one, so it walks 0 then 2, and the third
                // group no longer fits in the one remaining seat.
                1, 5,
                [
                    BookMyShowOp.Gather(2, 0, [0, 0]),
                    BookMyShowOp.Gather(2, 0, [0, 2]),
                    BookMyShowOp.Gather(2, 0, []),
                    BookMyShowOp.ScatterSeated(1, 0),
                    BookMyShowOp.ScatterNotSeated(1, 0),
                ]
            },
            {
                // Scatter spilling across rows: five seats over three rows of two
                // leaves exactly one free seat, in the last row.
                3, 2,
                [
                    BookMyShowOp.ScatterSeated(5, 2),
                    BookMyShowOp.Gather(1, 2, [2, 1]),
                    BookMyShowOp.ScatterNotSeated(1, 2),
                ]
            },
            {
                // maxRow really does bound the search: the same group that finds no
                // row at maxRow 0 is seated in row 1 as soon as maxRow allows it.
                3, 5,
                [
                    BookMyShowOp.Gather(5, 0, [0, 0]),
                    BookMyShowOp.Gather(5, 0, []),
                    BookMyShowOp.Gather(5, 1, [1, 0]),
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BookMyShowByRowScan_LeetCodeExamples_SeatsEachGroupInTheLowestQualifyingRow(
        int rowCount, int seatsPerRow, BookMyShowOp[] operations) =>
        RunScript(new BookingConcertTicketsInGroupsSolution.BookMyShowByRowScan(rowCount, seatsPerRow), operations);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BookMyShowBySegmentTreeBinarySearch_LeetCodeExamples_SeatsEachGroupInTheLowestQualifyingRow(
        int rowCount, int seatsPerRow, BookMyShowOp[] operations) =>
        RunScript(new BookingConcertTicketsInGroupsSolution.BookMyShowBySegmentTreeBinarySearch(rowCount, seatsPerRow), operations);

    private static void RunScript(
        BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy, BookMyShowOp[] operations)
    {
        foreach (var operation in operations)
        {
            operation.AssertAgainst(strategy);
        }
    }

    // One call in a BookMyShow script: which method to invoke, with what arguments,
    // and what LeetCode says it answers. Built via the named factories below so a
    // script (like Examples above) reads like the LeetCode call sequence it replays,
    // and so the answer a scatter is asserted against is named by the factory that
    // chose it rather than by a bare `bool` at the call site.
    public readonly record struct BookMyShowOp(
        bool isGather, int k, int maxRow, int[] expectedSeating, bool expectedSeated)
    {
        public static BookMyShowOp Gather(int k, int maxRow, int[] expected) => new(true, k, maxRow, expected, false);

        public static BookMyShowOp ScatterSeated(int k, int maxRow) => new(false, k, maxRow, [], true);

        public static BookMyShowOp ScatterNotSeated(int k, int maxRow) => new(false, k, maxRow, [], false);

        // Internal, not public: IBookMyShowStrategy is internal to
        // BookingConcertTicketsInGroupsSolution, and only this same assembly's
        // RunScript ever calls this.
        internal void AssertAgainst(BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy)
        {
            if (isGather)
            {
                var seating = strategy.Gather(k, maxRow);

                Assert.Equal(expectedSeating, seating);
                return;
            }

            var seated = strategy.Scatter(k, maxRow);

            Assert.Equal(expectedSeated, seated);
        }
    }
}
