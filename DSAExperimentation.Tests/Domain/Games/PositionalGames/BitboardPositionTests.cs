using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

public sealed partial class BitboardPositionTests
{
    [Fact]
    public void Claimed_IsEitherPlayersCells() =>
        Assert.Equal(0b1111UL, new BitboardPosition(0b0101, 0b1010, IsLost: false).Claimed);

    [Fact]
    public void MaskOf_SetsABitAtEveryListedCell() =>
        Assert.Equal((1UL << 63) | 0b1001UL, BitboardPosition.MaskOf([0, 3, 63]));
}
