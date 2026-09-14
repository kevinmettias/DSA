using static DSAExperimentation.LeetCode.BookingConcertTicketsInGroups.BookingConcertTicketsInGroupsSolution;

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
                    BookMyShowOp.Scatter(5, 1, true),
                    BookMyShowOp.Scatter(5, 1, false),
                ]
            },
            {
                // No row within maxRow is wide enough to seat the group at all.
                3, 3, [BookMyShowOp.Gather(4, 2, [])]
            },
            {
                // Two rows of three seats is six seats; seven never fits.
                2, 3, [BookMyShowOp.Scatter(7, 1, false)]
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
                    BookMyShowOp.Scatter(1, 0, true),
                    BookMyShowOp.Scatter(1, 0, false),
                ]
            },
            {
                // Scatter spilling across rows: five seats over three rows of two
                // leaves exactly one free seat, in the last row.
                3, 2,
                [
                    BookMyShowOp.Scatter(5, 2, true),
                    BookMyShowOp.Gather(1, 2, [2, 1]),
                    BookMyShowOp.Scatter(1, 2, false),
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
        RunScript(new BookMyShowByRowScan(rowCount, seatsPerRow), operations);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BookMyShowBySegmentTreeBinarySearch_LeetCodeExamples_SeatsEachGroupInTheLowestQualifyingRow(
        int rowCount, int seatsPerRow, BookMyShowOp[] operations) =>
        RunScript(new BookMyShowBySegmentTreeBinarySearch(rowCount, seatsPerRow), operations);

    private static void RunScript(IBookMyShowStrategy strategy, BookMyShowOp[] operations)
    {
        foreach (var operation in operations)
        {
            operation.AssertAgainst(strategy);
        }
    }
}

// One call in a BookMyShow script: which method to invoke, with what arguments, and
// what LeetCode says it answers. Built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct BookMyShowOp
{
    private readonly bool _isGather;
    private readonly int _k;
    private readonly int _maxRow;
    private readonly int[] _expectedSeating;
    private readonly bool _expectedSeated;

    private BookMyShowOp(bool isGather, int k, int maxRow, int[] expectedSeating, bool expectedSeated)
    {
        _isGather = isGather;
        _k = k;
        _maxRow = maxRow;
        _expectedSeating = expectedSeating;
        _expectedSeated = expectedSeated;
    }

    public static BookMyShowOp Gather(int k, int maxRow, int[] expected) => new(true, k, maxRow, expected, false);

    public static BookMyShowOp Scatter(int k, int maxRow, bool expected) => new(false, k, maxRow, [], expected);

    // Internal, not public: IBookMyShowStrategy is internal to
    // BookingConcertTicketsInGroupsSolution, and only this same assembly's
    // RunScript ever calls this.
    internal void AssertAgainst(IBookMyShowStrategy strategy)
    {
        if (_isGather)
        {
            Assert.Equal(_expectedSeating, strategy.Gather(_k, _maxRow));
            return;
        }

        Assert.Equal(_expectedSeated, strategy.Scatter(_k, _maxRow));
    }
}
