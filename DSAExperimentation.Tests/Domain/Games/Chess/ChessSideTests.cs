using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

// White faces up the board and Black down it, so each side's forward is the other's backward.
public sealed partial class ChessSideTests
{
    [Fact]
    public void Forward_WhiteIsUpAndBlackIsDown() =>
        Assert.Equal((GridStep.Up, GridStep.Down), (ChessSide.White.Forward, ChessSide.Black.Forward));

    [Fact]
    public void Forward_EachSidesIsTheOthersOpposite() =>
        Assert.Equal(-ChessSide.White.Forward, ChessSide.Black.Forward);
}
