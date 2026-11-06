using DSAExperimentation.Domain.Games.TicTacToe;

namespace DSAExperimentation.Tests.Domain.Games.TicTacToe;

// The shape is the fact; MnkGameTests lists its eight lines and solves the game as a draw.
public sealed partial class TicTacToeBoardTests
{
    [Fact]
    public void Columns_IsThree() => Assert.Equal(3, TicTacToeBoard.Columns);

    [Fact]
    public void Rows_IsThree() => Assert.Equal(3, TicTacToeBoard.Rows);

    [Fact]
    public void InARow_IsTheWholeSide() => Assert.Equal(TicTacToeBoard.Rows, TicTacToeBoard.InARow);
}
