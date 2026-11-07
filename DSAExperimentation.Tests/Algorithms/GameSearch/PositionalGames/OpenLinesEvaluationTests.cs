using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames;

// On Connect Four's board, rows numbered from the top. Three of one player's pieces in a run, with
// nothing else on the board, lie on three lines of four: one holds all three and lacks one cell,
// one holds two and lacks two, and one holds one, which counts for nothing. So a lone open three
// along any of the four line directions is worth one threat and one open two, and an evaluation
// that ignored a direction would find nothing there.
public sealed partial class OpenLinesEvaluationTests
{
    private const int LoneThree =
        OpenLinesEvaluation.OneShortWeight + OpenLinesEvaluation.TwoShortWeight;

    public static TheoryData<int[]> LoneThreesAlongEachDirection => new()
    {
        { [Cell(5, 0), Cell(5, 1), Cell(5, 2)] },
        { [Cell(5, 0), Cell(4, 0), Cell(3, 0)] },
        { [Cell(0, 0), Cell(1, 1), Cell(2, 2)] },
        { [Cell(0, 6), Cell(1, 5), Cell(2, 4)] },
    };

    [Fact]
    public void Evaluate_EmptyBoard_IsNothing() =>
        Assert.Equal(0, Evaluation().Evaluate(default));

    [Theory]
    [MemberData(nameof(LoneThreesAlongEachDirection))]
    public void Evaluate_ALoneThreeOfTheMover_IsAThreatAndAnOpenTwo(int[] cells) =>
        Assert.Equal(LoneThree, Evaluation().Evaluate(MoverHolding(cells)));

    [Theory]
    [MemberData(nameof(LoneThreesAlongEachDirection))]
    public void Evaluate_ALoneThreeOfTheOpponent_CountsAgainstTheMover(int[] cells) =>
        Assert.Equal(-LoneThree, Evaluation().Evaluate(OpponentHolding(cells)));

    [Fact]
    public void Evaluate_TwoInARow_IsAnOpenTwo() =>
        Assert.Equal(
            OpenLinesEvaluation.TwoShortWeight,
            Evaluation().Evaluate(MoverHolding([Cell(5, 0), Cell(5, 1)])));

    // The opponent's piece beside the mover's two kills both lines of four that hold either, and
    // every other line holds a single piece.
    [Fact]
    public void Evaluate_LinesBothPlayersHold_CountForNobody()
    {
        var mover = BitboardPosition.MaskOf([Cell(5, 0), Cell(5, 1)]);
        var opponent = BitboardPosition.MaskOf([Cell(5, 2)]);

        Assert.Equal(0, Evaluation().Evaluate(new(mover, opponent, IsLost: false)));
    }

    [Theory]
    [MemberData(nameof(LoneThreesAlongEachDirection))]
    public void Evaluate_EachLoneThreeMirrored_IsWorthTheSame(int[] cells) =>
        Assert.Equal(
            Evaluation().Evaluate(MoverHolding(cells)),
            Evaluation().Evaluate(MoverHolding([.. cells.Select(Mirrored)])));

    private static OpenLinesEvaluation Evaluation() => new(MnkGame<ConnectFourBoard>.Lines);

    private static int Cell(int row, int column) => MnkGame<ConnectFourBoard>.CellAt(row, column);

    // The cell in the same row, as far from the right edge as this one is from the left.
    private static int Mirrored(int cell)
    {
        var row = cell / ConnectFourBoard.Columns;
        var column = cell % ConnectFourBoard.Columns;

        return Cell(row, ConnectFourBoard.Columns - 1 - column);
    }

    private static BitboardPosition MoverHolding(int[] cells) =>
        new(BitboardPosition.MaskOf(cells), 0, IsLost: false);

    private static BitboardPosition OpponentHolding(int[] cells) =>
        new(0, BitboardPosition.MaskOf(cells), IsLost: false);
}
