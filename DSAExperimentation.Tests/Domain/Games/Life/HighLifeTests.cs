using DSAExperimentation.Domain.Games.Life;

namespace DSAExperimentation.Tests.Domain.Games.Life;

public sealed partial class HighLifeTests
{
    [Fact]
    public void BirthMask_IsBThreeSix() => Assert.Equal(0b100_1000, HighLife.BirthMask);

    [Fact]
    public void SurvivalMask_IsConwaysSTwoThree() => Assert.Equal(ConwayLife.SurvivalMask, HighLife.SurvivalMask);
}
