using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

public sealed partial class FreePlacementTests
{
    private const int Cells = 9;
    private const int Move = 5;

    public static TheoryData<int> MovesOffTheBoard => new() { -1, Cells };

    [Fact]
    public void TryPlace_AMoveOnTheBoard_ClaimsTheCellItNames() =>
        Assert.Equal((true, Move), (FreePlacement.TryPlace(Empty(), Move, out var cell), cell));

    [Theory]
    [MemberData(nameof(MovesOffTheBoard))]
    public void TryPlace_AMoveOffTheBoard_IsRefused(int move) =>
        Assert.False(FreePlacement.TryPlace(Empty(), move, out _));

    [Fact]
    public void Choices_AreTheOpenCells() =>
        Assert.Equal(0b1010_0110UL, FreePlacement.Choices(0b1010_0110UL));

    [Fact]
    public void CellClaimed_IsTheCellChosen() =>
        Assert.Equal(Move, FreePlacement.CellClaimed(1UL << Move, Move));

    private static int[] Empty() => BoardMask.Full(Cells).EmptyPosition();
}
