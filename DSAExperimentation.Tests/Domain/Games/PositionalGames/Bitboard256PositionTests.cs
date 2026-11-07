using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

public sealed partial class Bitboard256PositionTests
{
    [Fact]
    public void Claimed_IsEitherPlayersCells() =>
        Assert.Equal(
            Bitboard256.Of([3, 100, 200]),
            new Bitboard256Position(Bitboard256.Of([3, 200]), Bitboard256.Cell(100), IsLost: false)
                .Claimed);
}
