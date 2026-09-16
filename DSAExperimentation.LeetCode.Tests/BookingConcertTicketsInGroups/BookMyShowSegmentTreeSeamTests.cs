using DSAExperimentation.LeetCode.BookingConcertTicketsInGroups;

namespace DSAExperimentation.LeetCode.Tests.BookingConcertTicketsInGroups;

// The seam between BookingConcertTicketsInGroupsSolution's two seating strategies.
// BookMyShowByRowScan keeps one int[] of remaining seats per row and is the problem
// statement read literally; BookMyShowBySegmentTreeBinarySearch replaces that array
// with two DataStructures.SegmentTree instances - one under MaxOperation<int>, one
// under SumOperation<int> - plus BinarySearch.LowerBound over the prefix-max
// predicate HasCapacitySequence claims is monotone.
//
// Everything that can go wrong here is between the two trees: Gather writes both
// through SetAvailable, Scatter writes both per row it touches, and no single tree
// can notice if the other stopped agreeing with it. The raw arm is the only
// reference that can, so every test below replays one booking lifecycle against
// both arms and asserts they answer identically. The published-example suite pins
// each arm separately; it cannot see a call where they diverge.
public sealed partial class BookMyShowSegmentTreeSeamTests
{
    private const int RowCount = 3;
    private const int SeatsPerRow = 5;

    [Fact]
    public void Gather_DrainsEveryRowInTurn_MatchesRowScan()
    {
        SeatRequest[] script =
        [
            SeatRequest.Gather(4, 2),
            SeatRequest.Gather(4, 2),
            SeatRequest.Gather(2, 2),
            SeatRequest.Gather(2, 2),
            SeatRequest.Gather(2, 2),
            SeatRequest.Gather(1, 2),
        ];

        AssertSameAnswers(script);
    }

    [Fact]
    public void Scatter_StaysWithinMaxRow_MatchesRowScan()
    {
        SeatRequest[] script =
        [
            SeatRequest.Scatter(3, 1),
            SeatRequest.Scatter(4, 2),
            SeatRequest.Scatter(2, 0),
            SeatRequest.Scatter(6, 2),
        ];

        AssertSameAnswers(script);
    }

    [Fact]
    public void GatherThenScatter_OnTheSamePartialRows_MatchesRowScan()
    {
        SeatRequest[] script =
        [
            SeatRequest.Scatter(4, 1),
            SeatRequest.Gather(3, 2),
            SeatRequest.Gather(2, 1),
            SeatRequest.Scatter(5, 2),
            SeatRequest.Gather(1, 2),
        ];

        AssertSameAnswers(script);
    }

    [Fact]
    public void Gather_GroupWiderThanEveryRow_AnswersEmptyOnBothArms()
    {
        SeatRequest[] script =
        [
            SeatRequest.Gather(6, 2),
            SeatRequest.Gather(6, 2),
        ];

        AssertSameAnswers(script);
    }

    // maxRow is inclusive and the search runs from row 0, so row 0 alone must be able
    // to satisfy a Gather whose maxRow is 0 - the one case where the prefix-max
    // predicate HasCapacitySequence builds has length 1 and BinarySearch.LowerBound
    // can only return 0 or that length.
    [Fact]
    public void Gather_LimitedToTheFirstRow_MatchesRowScan()
    {
        SeatRequest[] script =
        [
            SeatRequest.Gather(3, 0),
            SeatRequest.Gather(3, 0),
            SeatRequest.Gather(1, 0),
            SeatRequest.Gather(1, 0),
        ];

        AssertSameAnswers(script);
    }

    // A hall of exactly one row: Scatter can never spill past it, so the sum tree's
    // range query and the per-row writes have to agree about a window of length 1.
    [Fact]
    public void Scatter_SingleRowHall_MatchesRowScan()
    {
        SeatRequest[] script =
        [
            SeatRequest.Scatter(5, 0),
            SeatRequest.Scatter(1, 0),
            SeatRequest.Gather(2, 0),
        ];

        AssertSameAnswers(script, rowCount: 1, seatsPerRow: 5);
    }

    private static void AssertSameAnswers(SeatRequest[] script, int rowCount, int seatsPerRow)
        => Assert.Equal(
            Replay(ReferenceArm(rowCount, seatsPerRow), script),
            Replay(ComposedArm(rowCount, seatsPerRow), script));

    private static void AssertSameAnswers(SeatRequest[] script) => AssertSameAnswers(script, RowCount, SeatsPerRow);

    private static BookingConcertTicketsInGroupsSolution.BookMyShowByRowScan ReferenceArm(
        int rowCount, int seatsPerRow) => new(rowCount, seatsPerRow);

    private static BookingConcertTicketsInGroupsSolution.BookMyShowBySegmentTreeBinarySearch ComposedArm(
        int rowCount, int seatsPerRow) => new(rowCount, seatsPerRow);

    private static string Replay(
        BookingConcertTicketsInGroupsSolution.IBookMyShowStrategy strategy, SeatRequest[] script)
    {
        var answers = new List<string>(script.Length);

        foreach (var request in script)
        {
            answers.Add(request.IsGather
                ? $"gather[{string.Join(',', strategy.Gather(request.GroupSize, request.MaxRow))}]"
                : $"scatter[{strategy.Scatter(request.GroupSize, request.MaxRow)}]");
        }

        return string.Join(' ', answers);
    }

    private readonly record struct SeatRequest(bool IsGather, int GroupSize, int MaxRow)
    {
        public static SeatRequest Gather(int groupSize, int maxRow) => new(IsGather: true, groupSize, maxRow);

        public static SeatRequest Scatter(int groupSize, int maxRow) => new(IsGather: false, groupSize, maxRow);
    }
}
