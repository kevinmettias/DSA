using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames;

// The same rule as OpenLinesEvaluation, counted by shifts: on Connect Four's board, which both
// forms hold, the two agree on positions drawn at random at every density; on Gomoku's, the count
// is held to the lines read one at a time (LineOracle) and weighed the same way.
public sealed partial class ShiftedOpenLinesEvaluationTests
{
    private const int RandomPositions = 100;
    private const int Seed = 1950;
    private const int LoneThreeInARow = 3 * OpenLinesEvaluation.TwoShortWeight;

    public static TheoryData<int> Densities => new() { 10, 30, 50, 70 };

    [Fact]
    public void Evaluate_EmptyBoard_IsNothing() =>
        Assert.Equal(0, Gomoku().Evaluate(default));

    // Three in the middle of row 7: three lines of five hold all three, each two short, and every
    // other line holds one or two, too short to count.
    [Fact]
    public void Evaluate_ALoneThreeOfTheMover_IsThreeLinesTwoShort() =>
        Assert.Equal(LoneThreeInARow, Gomoku().Evaluate(MoverHolding(LoneThree())));

    [Fact]
    public void Evaluate_TheSameThreeOfTheOpponent_CountsAgainstTheMover()
    {
        var three = MoverHolding(LoneThree());

        Assert.Equal(
            -LoneThreeInARow, Gomoku().Evaluate(new(three.Opponent, three.Mover, IsLost: false)));
    }

    [Theory]
    [MemberData(nameof(Densities))]
    public void Evaluate_ConnectFourBoard_IsOpenLinesEvaluation(int percent)
    {
        var board = MnkGame<ConnectFourBoard>.Board;
        var template = new OpenLinesEvaluation(MnkGame<ConnectFourBoard>.Lines);
        var shifted = new ShiftedOpenLinesEvaluation<ConnectFourBoard, KOrMore>(new(board));
        var random = new Random(Seed + percent);

        Assert.All(Enumerable.Range(0, RandomPositions), _ =>
        {
            var (mine, theirs) = LineOracle<ConnectFourBoard, KOrMore>.RandomCells(board, random, percent);
            var (moverCells, opponentCells) = (
                LineOracle<ConnectFourBoard, KOrMore>.CellsOf(mine),
                LineOracle<ConnectFourBoard, KOrMore>.CellsOf(theirs));
            var small = new BitboardPosition(
                BitboardPosition.MaskOf(moverCells),
                BitboardPosition.MaskOf(opponentCells),
                IsLost: false);
            var wide = new Bitboard256Position(
                InARowLines<ConnectFourBoard, KOrMore>.MaskOf(moverCells),
                InARowLines<ConnectFourBoard, KOrMore>.MaskOf(opponentCells),
                IsLost: false);

            Assert.Equal(template.Evaluate(small), shifted.Evaluate(wide));
        });
    }

    [Theory]
    [MemberData(nameof(Densities))]
    public void Evaluate_GomokuBoard_WeighsTheLinesOneAtATime(int percent)
    {
        var board = MnkGame<GomokuBoard>.Board;
        var random = new Random(Seed + percent);

        Assert.All(Enumerable.Range(0, RandomPositions), _ =>
        {
            var (mine, theirs) = LineOracle<GomokuBoard, KOrMore>.RandomCells(board, random, percent);
            var expected = Weighed(LineOracle<GomokuBoard, KOrMore>.CountLines(board, mine, theirs))
                - Weighed(LineOracle<GomokuBoard, KOrMore>.CountLines(board, theirs, mine));

            Assert.Equal(expected, Gomoku().Evaluate(Position(mine, theirs)));
        });
    }

    private static ShiftedOpenLinesEvaluation<GomokuBoard, KOrMore> Gomoku() =>
        new(new(MnkGame<GomokuBoard>.Board));

    private static int[] LoneThree() =>
        [.. Enumerable.Range(6, 3).Select(column => MnkGame<GomokuBoard>.CellAt(7, column))];

    private static Bitboard256Position MoverHolding(int[] cells) =>
        new(InARowLines<GomokuBoard, KOrMore>.MaskOf(cells), Bitboard256.Empty, IsLost: false);

    private static Bitboard256Position Position(bool[] mine, bool[] theirs) =>
        new(
            InARowLines<GomokuBoard, KOrMore>.MaskOf(LineOracle<GomokuBoard, KOrMore>.CellsOf(mine)),
            InARowLines<GomokuBoard, KOrMore>.MaskOf(LineOracle<GomokuBoard, KOrMore>.CellsOf(theirs)),
            IsLost: false);

    // Lines of five one short and two short, at OpenLinesEvaluation's weights.
    private static int Weighed(int[] byHeld) =>
        (OpenLinesEvaluation.OneShortWeight * byHeld[GomokuBoard.InARow - 1])
        + (OpenLinesEvaluation.TwoShortWeight * byHeld[GomokuBoard.InARow - 2]);
}
