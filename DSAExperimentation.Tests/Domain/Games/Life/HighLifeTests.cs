using DSAExperimentation.Domain.Games.Life;

namespace DSAExperimentation.Tests.Domain.Games.Life;

public sealed partial class HighLifeTests
{
    [Fact]
    public void Rulestring_IsBThreeSixSTwoThree() => Assert.Equal("B36/S23", HighLife.Rulestring);
}
