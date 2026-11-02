using DSAExperimentation.Domain.Games.Dice;

namespace DSAExperimentation.Tests.Domain.Games.Dice;

public sealed partial class DieTests
{
    [Fact]
    public void LowestFace_SixSidedDie_IsOne() => Assert.Equal(1, Die<SixSidedDie>.LowestFace);

    [Fact]
    public void HighestFace_SixSidedDie_IsSix() => Assert.Equal(6, Die<SixSidedDie>.HighestFace);

    // A die numbered from 0 ends one lower than its face count.
    [Fact]
    public void HighestFace_TenSidedDieFromZero_IsNine() => Assert.Equal(9, Die<TenSidedFromZero>.HighestFace);

    private readonly struct TenSidedFromZero : IDie
    {
        public static int LowestFace => 0;

        public static int FaceCount => 10;
    }
}
