using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames;

// Victory by continuous fours on Gomoku's board, on positions drawn by hand (GomokuPositions), X to
// move. Every sequence the search returns is replayed against MnkGame's lines read one at a time
// (ForcedWinReplay), not against the shifts the search reads: before each reply of the defender's,
// the defender has no winning cell and the attacker has one, and when the attacker has exactly one
// the reply is it; and the attacker's last move completes a run that wins.
public sealed partial class ContinuousFoursTests
{
    public static TheoryData<string[]> PositionsWithAWin => new()
    {
        { GomokuPositions.DoubleFour },
        { GomokuPositions.BrokenFour },
        { GomokuPositions.CounterFourBlockedByAFour },
        { GomokuPositions.SixFoursDeep },
    };

    public static TheoryData<string[]> PositionsWithNoWin => new()
    {
        { GomokuPositions.CounterFour },
        { GomokuPositions.EmptyBoard },
    };

    // Each position with a win, and how many plies its shortest one takes.
    public static TheoryData<string[], int> ShortestWins => new()
    {
        { GomokuPositions.BrokenFour, 1 },
        { GomokuPositions.DoubleFour, 3 },
        { GomokuPositions.CounterFourBlockedByAFour, 5 },
        { GomokuPositions.SixFoursDeep, 13 },
    };

    [Theory]
    [MemberData(nameof(PositionsWithAWin))]
    public void TryFindWin_AForcedWin_IsASequenceOfForcedReplies(string[] rows) =>
        Assert.Equal([], Problems<KOrMore>(rows));

    [Theory]
    [MemberData(nameof(PositionsWithAWin))]
    public void TryFindWin_AForcedWinUnderExactlyFive_IsASequenceOfForcedReplies(string[] rows) =>
        Assert.Equal([], Problems<ExactlyK>(rows));

    [Theory]
    [MemberData(nameof(PositionsWithNoWin))]
    public void TryFindWin_NoForcedWin_FindsNone(string[] rows) =>
        Assert.Equal((false, 0), Search<KOrMore>(rows));

    // The search deepens a four at a time, so the win it returns is a shortest one.
    [Theory]
    [MemberData(nameof(ShortestWins))]
    public void TryFindWin_AForcedWin_IsAShortestOne(string[] rows, int plies) =>
        Assert.Equal((true, plies), Search<KOrMore>(rows));

    [Fact]
    public void TryFindWin_ABrokenFour_WinsInTheGap() =>
        Assert.Equal([GomokuDiagram.Cell(7, 5)], Sequence<KOrMore>(GomokuPositions.BrokenFour));

    // The counter-four is answered first: X's second move takes O's winning cell (6,8).
    [Fact]
    public void TryFindWin_ADefendersCounterFour_IsBlockedByTheAttackersNextFour() =>
        Assert.Equal(
            GomokuDiagram.Cell(6, 8),
            Sequence<KOrMore>(GomokuPositions.CounterFourBlockedByAFour)[2]);

    [Fact]
    public void TryFindWin_OnlyAnOverline_WinsInFreestyle() =>
        Assert.Equal([GomokuDiagram.Cell(7, 4)], Sequence<KOrMore>(GomokuPositions.OnlyAnOverline));

    [Fact]
    public void TryFindWin_OnlyAnOverline_IsNoWinUnderExactlyFive() =>
        Assert.Equal((false, 0), Search<ExactlyK>(GomokuPositions.OnlyAnOverline));

    [Fact]
    public void TryFindWin_AGameAlreadyLost_FindsNone()
    {
        var lost = GomokuDiagram.Position(GomokuPositions.BrokenFour) with { IsLost = true };

        Assert.False(ContinuousFours.TryFindWin(Game<KOrMore>(), lost, out _));
    }

    private static InARowGame<GomokuBoard, TOverline> Game<TOverline>()
        where TOverline : struct, IOverlineRule
        => new(MnkGame<GomokuBoard>.Board);

    private static (bool Found, int Length) Search<TOverline>(string[] rows)
        where TOverline : struct, IOverlineRule
    {
        var position = GomokuDiagram.Position(rows);
        var isFound = ContinuousFours.TryFindWin(Game<TOverline>(), position, out var sequence);

        return (isFound, sequence.Length);
    }

    private static int[] Sequence<TOverline>(string[] rows)
        where TOverline : struct, IOverlineRule
    {
        var position = GomokuDiagram.Position(rows);
        ContinuousFours.TryFindWin(Game<TOverline>(), position, out var sequence);

        return sequence;
    }

    // What is wrong with the sequence the search returns, read off the lines one at a time.
    private static string[] Problems<TOverline>(string[] rows)
        where TOverline : struct, IOverlineRule
    {
        var position = GomokuDiagram.Position(rows);

        if (!ContinuousFours.TryFindWin(Game<TOverline>(), position, out var sequence))
        {
            return ["no win found"];
        }

        return new ForcedWinReplay<TOverline>(rows, sequence).Problems();
    }

    // The search's own steps, driven through TryFindWin: whether the attacker wins, and the cells
    // of the line it found, which is empty when it found none.
    public sealed partial class FourSearchTests
    {
        [Fact]
        public void IsWon_SixFoursDeep_IsTrue() =>
            Assert.True(Search<KOrMore>(GomokuPositions.SixFoursDeep).Found);

        [Fact]
        public void Sequence_NoWin_IsEmpty() =>
            Assert.Empty(Sequence<KOrMore>(GomokuPositions.CounterFour));
    }
}
