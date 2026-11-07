using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

public sealed partial class NoCutoffTests
{
    [Fact]
    public void IsCutoff_EvenWhenAlphaPassesBeta_IsNever() =>
        Assert.False(NoCutoff<int>.IsCutoff(3, 2));
}
