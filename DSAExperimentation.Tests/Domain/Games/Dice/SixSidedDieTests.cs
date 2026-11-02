using DSAExperimentation.Domain.Games.Dice;

namespace DSAExperimentation.Tests.Domain.Games.Dice;

// The faces are the fact itself, so the tests state them.
public sealed partial class SixSidedDieTests
{
    [Fact]
    public void LowestFace_IsOne() => Assert.Equal(1, SixSidedDie.LowestFace);

    [Fact]
    public void FaceCount_IsSix() => Assert.Equal(6, SixSidedDie.FaceCount);
}
