using static DSAExperimentation.LeetCode.SeatReservationManager.SeatReservationManagerSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SeatReservationManager;

// Harness only: both strategies live in SeatReservationManagerSolution. LeetCode's
// own shape here is a stateful object across a sequence of calls, so Examples
// encodes a call script instead of a single argument tuple - the same shape
// DesignAuthenticationManagerTests uses for its own instance-API problem.
// unreserve returns null in LeetCode's judge output, so SeatManagerOp returns null
// for it too and the expected sequence reads exactly like the published one.
//
// The bool[]-scan baseline used to exist only as a benchmark arm with nothing
// asserting it; it is pinned to the same scripts here, which is what puts the seat
// count in the data at all - it is the one strategy that really does allocate an
// entry per seat up front.
public sealed class SeatReservationManagerTests
{
    public static TheoryData<SeatManagerScript, int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example.
                new SeatManagerScript(
                    SeatCount: 5,
                    [
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Unreserve(2),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Unreserve(5),
                    ]),
                [1, 2, null, 2, 3, 4, 5, null]
            },
            {
                // The script the pre-section-17 test asserted: LeetCode's example
                // continued two calls further, so the seat released last is
                // reissued before a never-issued one is minted. Six seats, because
                // the second of those calls issues one past the published five.
                new SeatManagerScript(
                    SeatCount: 6,
                    [
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Unreserve(2),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Unreserve(5),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                    ]),
                [1, 2, null, 2, 3, 4, 5, null, 5, 6]
            },
            {
                // Three seats released out of ascending order, so the reissue order
                // is decided by the release store rather than by the release
                // sequence - the case the original script never reached, since it
                // never held more than one released seat at a time.
                new SeatManagerScript(
                    SeatCount: 6,
                    [
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Unreserve(4),
                        SeatManagerOp.Unreserve(1),
                        SeatManagerOp.Unreserve(3),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                        SeatManagerOp.Reserve(),
                    ]),
                [1, 2, 3, 4, 5, null, null, null, 1, 3, 4, 6]
            },
            {
                // A single seat, taken, handed back and taken again.
                new SeatManagerScript(
                    SeatCount: 1,
                    [SeatManagerOp.Reserve(), SeatManagerOp.Unreserve(1), SeatManagerOp.Reserve()]),
                [1, null, 1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SeatManagerByLinearScanArray_LeetCodeExamples_MatchesExpectedSequence(
        SeatManagerScript script, int?[] expected) =>
        RunScript(new SeatManagerByLinearScanArray(script.SeatCount), script.Operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void SeatManagerByReleasedSeatHeap_LeetCodeExamples_MatchesExpectedSequence(
        SeatManagerScript script, int?[] expected) =>
        RunScript(new SeatManagerByReleasedSeatHeap(), script.Operations, expected);

    private static void RunScript(ISeatManager manager, SeatManagerOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(manager));
        }
    }
}

// One LeetCode call script: the seat count its constructor was given, and the calls
// made after it. The two travel together because only one of the strategies takes
// the count - the lazy-release heap never allocates per seat, so it answers the
// identical script without being told how many seats exist.
public readonly record struct SeatManagerScript(int SeatCount, SeatManagerOp[] Operations);

// One call in a SeatManager script: which operation to invoke, and for unreserve,
// on which seat. Pure dispatch, built via the named factories below so a script
// reads like the LeetCode call sequence it replays.
public readonly record struct SeatManagerOp
{
    private readonly Kind _kind;
    private readonly int _seatNumber;

    private SeatManagerOp(Kind kind, int seatNumber)
    {
        _kind = kind;
        _seatNumber = seatNumber;
    }

    public static SeatManagerOp Reserve() => new(Kind.Reserve, seatNumber: 0);

    public static SeatManagerOp Unreserve(int seatNumber) => new(Kind.Unreserve, seatNumber);

    // null for unreserve, matching LeetCode's own judge output, so a script runner
    // can assert against one expected value per operation uniformly.
    internal int? Apply(ISeatManager manager)
    {
        if (_kind == Kind.Reserve)
        {
            return manager.Reserve();
        }

        manager.Unreserve(_seatNumber);
        return null;
    }

    private enum Kind
    {
        Reserve,
        Unreserve,
    }
}
