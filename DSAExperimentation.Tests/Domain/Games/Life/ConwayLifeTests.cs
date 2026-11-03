using DSAExperimentation.Domain.Games.Life;

namespace DSAExperimentation.Tests.Domain.Games.Life;

public sealed partial class ConwayLifeTests
{
    [Fact]
    public void Rulestring_IsBThreeSTwoThree() => Assert.Equal("B3/S23", ConwayLife.Rulestring);
}
