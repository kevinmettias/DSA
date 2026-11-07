using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using FreestyleGomoku = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.KOrMore>;
using StandardGomoku = DSAExperimentation.Domain.Games.PositionalGames.InARow.InARowGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.GomokuBoard,
    DSAExperimentation.Domain.Games.PositionalGames.InARow.ExactlyK>;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// The m,n,k game on 256-bit masks: the layer's contract on the 7,6,4 board beside BitboardGame
// (ContractTests), and what only this form has, Gomoku's 225 cells and standard gomoku's exactly
// five, rows numbered from the top. Negamax over the masks gives tic-tac-toe and the 4,3,3 game
// their known values.
public sealed partial class InARowGameTests
{
    private const int Draw = 0;
    private const int Row = 7;

    [Fact]
    public void MoveCount_GomokuEmptyBoard_IsEveryCell() =>
        Assert.Equal(MnkGame<GomokuBoard>.CellCount, Freestyle().MoveCount(default));

    [Fact]
    public void OpenCells_AfterTwoMoves_LeavesTheirCellsOut() =>
        Assert.Equal(
            MnkGame<GomokuBoard>.CellCount - 2,
            Freestyle().OpenCells(Freestyle().Replay([0, 224])).Count);

    // The corner and the cell after it taken: move 0 is the second cell of row 0, and the last
    // move the last cell of the board.
    [Fact]
    public void ChoiceOf_FirstAndLastMove_AreTheLowestAndHighestOpenCells()
    {
        var game = Freestyle();
        var position = game.Replay([0, 1]);
        var last = game.MoveCount(position) - 1;

        Assert.Equal(
            (2, MnkGame<GomokuBoard>.CellCount - 1),
            (game.ChoiceOf(position, 0), game.ChoiceOf(position, last)));
    }

    // A move puts the mover's stone on its cell and hands the move over: the stone is the
    // opponent's in the position after.
    [Fact]
    public void Next_TheCentre_IsTheOtherPlayersToMove()
    {
        var game = Freestyle();
        var centre = MnkGame<GomokuBoard>.CellAt(Row, Row);
        var after = game.Next(default, centre);

        Assert.Equal(
            (Bitboard256.Empty, InARowLines<GomokuBoard, KOrMore>.MaskOf([centre])),
            (after.Mover, after.Opponent));
    }

    [Fact]
    public void Gain_IsNothing() =>
        Assert.Equal(0, Freestyle().Gain(default, 0));

    // Six in a row along a row, the second player answering on the bottom row: freestyle ends the
    // game on five or more, standard gomoku only on a five with no stone of the player's beside it.
    [Fact]
    public void Replay_FiveInARow_EndsFreestyle() =>
        Assert.True(Freestyle().Replay(Moves([2, 3, 4, 5, 6])).IsLost);

    [Fact]
    public void Replay_ASixMadeInTheGap_DoesNotEndStandardGomoku() =>
        Assert.False(Standard().Replay(Moves([2, 3, 4, 6, 7, 5])).IsLost);

    [Fact]
    public void Replay_ASixMadeInTheGap_EndsFreestyle() =>
        Assert.True(Freestyle().Replay(Moves([2, 3, 4, 6, 7, 5])).IsLost);

    [Fact]
    public void Replay_AnExactFive_EndsStandardGomoku() =>
        Assert.True(Standard().Replay(Moves([2, 3, 4, 5, 6])).IsLost);

    [Fact]
    public void ValueWhenOver_AWinOnGomoku_IsTheClaimedCellsLessWinValue()
    {
        var game = Freestyle();
        var won = game.Replay(Moves([2, 3, 4, 5, 6]));

        Assert.Equal(9 - FreestyleGomoku.WinValue, game.ValueWhenOver(won));
    }

    [Fact]
    public void Negamax_TicTacToe_IsADraw() =>
        Assert.Equal(Draw, Solved<TicTacToeBoard>());

    [Fact]
    public void Negamax_FourByThreeThreeInARow_IsAFirstPlayerWin() =>
        Assert.True(Solved<FourByThreeBoard>() > Draw);

    [Fact]
    public void WinValue_IsTheSmallBoardsWinValue() =>
        Assert.Equal(BitboardGame<FreePlacement>.WinValue, FreestyleGomoku.WinValue);

    private static FreestyleGomoku Freestyle() => new(MnkGame<GomokuBoard>.Board);

    private static StandardGomoku Standard() => new(MnkGame<GomokuBoard>.Board);

    // The first player's stones at these columns of row 7, in order, the second player answering
    // at the same columns of the bottom row.
    private static int[] Moves(int[] columns)
    {
        var moves = new List<int>();

        foreach (var column in columns[..^1])
        {
            moves.AddRange([Cell(Row, column), Cell(GomokuBoard.Size - 1, column)]);
        }

        moves.Add(Cell(Row, columns[^1]));

        return [.. moves];
    }

    private static int Cell(int row, int column) => MnkGame<GomokuBoard>.CellAt(row, column);

    private static int Solved<TShape>()
        where TShape : struct, IMnkShape
        => Negamax.Value<InARowGame<TShape, KOrMore>, Bitboard256Position, int>(
            new(MnkGame<TShape>.Board), default);

    // The layer's contract on Connect Four's board under free placement, judged by shifts.
    public sealed partial class ContractTests
        : BitboardGameContractTests<
            ConnectFourBoard, InARowGame<ConnectFourBoard, KOrMore>, Bitboard256Position>
    {
        protected override InARowGame<ConnectFourBoard, KOrMore> GameOn(BoardMask board) =>
            new(board);
    }
}
