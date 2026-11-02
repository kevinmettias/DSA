using DSAExperimentation.Domain.Games.Zuma;

namespace DSAExperimentation.Tests.Domain.Games.Zuma;

public sealed partial class ClassicZumaTests
{
    [Fact]
    public void ShortestVanishingGroup_IsThree() => Assert.Equal(3, ClassicZuma.ShortestVanishingGroup);
}
