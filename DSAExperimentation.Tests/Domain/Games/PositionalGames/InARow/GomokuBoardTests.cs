using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// The shape is the fact; MnkGameTests counts its lines.
public sealed partial class GomokuBoardTests
{
    [Fact]
    public void Columns_IsFifteen() => Assert.Equal(15, GomokuBoard.Columns);

    [Fact]
    public void Rows_IsFifteen() => Assert.Equal(15, GomokuBoard.Rows);

    [Fact]
    public void InARow_IsFive() => Assert.Equal(5, GomokuBoard.InARow);
}
