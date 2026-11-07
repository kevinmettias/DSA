using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

public sealed partial class AlphaBetaCutoffTests
{
    public static TheoryData<int, int> AlphaAtOrPastBeta => new() { { 2, 2 }, { 3, 2 } };

    [Theory]
    [MemberData(nameof(AlphaAtOrPastBeta))]
    public void IsCutoff_AlphaReachingBeta_Cuts(int alpha, int beta) =>
        Assert.True(AlphaBetaCutoff<int>.IsCutoff(alpha, beta));

    [Fact]
    public void IsCutoff_AlphaBelowBeta_SearchesOn() =>
        Assert.False(AlphaBetaCutoff<int>.IsCutoff(1, 2));
}
