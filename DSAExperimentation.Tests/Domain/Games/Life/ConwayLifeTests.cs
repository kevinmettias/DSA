using DSAExperimentation.Domain.Games.Life;

namespace DSAExperimentation.Tests.Domain.Games.Life;

public sealed partial class ConwayLifeTests
{
    [Fact]
    public void BirthMask_IsBThree() => Assert.Equal(0b1000, ConwayLife.BirthMask);

    [Fact]
    public void SurvivalMask_IsSTwoThree() => Assert.Equal(0b1100, ConwayLife.SurvivalMask);
}
