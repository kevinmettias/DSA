using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// The shape is the fact; MnkGameTests counts its 69 lines, and GravityTests drops pieces on it.
public sealed partial class ConnectFourBoardTests
{
    [Fact]
    public void Columns_IsSeven() => Assert.Equal(7, ConnectFourBoard.Columns);

    [Fact]
    public void Rows_IsSix() => Assert.Equal(6, ConnectFourBoard.Rows);

    [Fact]
    public void InARow_IsFour() => Assert.Equal(4, ConnectFourBoard.InARow);
}
