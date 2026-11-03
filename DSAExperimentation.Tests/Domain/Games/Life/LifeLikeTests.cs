using DSAExperimentation.Domain.Games.Life;

namespace DSAExperimentation.Tests.Domain.Games.Life;

// The rule is its truth table over every count a cell can have, 0 through 8, shown here for Conway's.
public sealed partial class LifeLikeTests
{
    public static TheoryData<int> ConwayBirths => new() { 3 };

    public static TheoryData<int> ConwayNonBirths => new() { 0, 1, 2, 4, 5, 6, 7, 8 };

    public static TheoryData<int> ConwaySurvivals => new() { 2, 3 };

    public static TheoryData<int> ConwayDeaths => new() { 0, 1, 4, 5, 6, 7, 8 };

    [Theory]
    [MemberData(nameof(ConwayBirths))]
    public void IsBirthCount_ConwaysBirthCount_IsTrue(int liveNeighbors) =>
        Assert.True(LifeLike<ConwayLife>.IsBirthCount(liveNeighbors));

    [Theory]
    [MemberData(nameof(ConwayNonBirths))]
    public void IsBirthCount_AnyOtherCount_IsFalse(int liveNeighbors) =>
        Assert.False(LifeLike<ConwayLife>.IsBirthCount(liveNeighbors));

    [Theory]
    [MemberData(nameof(ConwaySurvivals))]
    public void IsSurvivalCount_ConwaysSurvivalCount_IsTrue(int liveNeighbors) =>
        Assert.True(LifeLike<ConwayLife>.IsSurvivalCount(liveNeighbors));

    [Theory]
    [MemberData(nameof(ConwayDeaths))]
    public void IsSurvivalCount_AnyOtherCount_IsFalse(int liveNeighbors) =>
        Assert.False(LifeLike<ConwayLife>.IsSurvivalCount(liveNeighbors));

    // HighLife differs from Conway's rule only in a birth on six.
    [Fact]
    public void IsBirthCount_HighLifeOnSix_IsTrueWhereConwaysIsFalse()
    {
        Assert.True(LifeLike<HighLife>.IsBirthCount(6));
        Assert.False(LifeLike<ConwayLife>.IsBirthCount(6));
    }

    // Seeds, B2/S, names no survival count, so no live cell survives.
    [Fact]
    public void IsSurvivalCount_RuleWithNoSurvivalCounts_IsFalseForEveryCount() =>
        Assert.All(Enumerable.Range(0, 9), liveNeighbors => Assert.False(LifeLike<Seeds>.IsSurvivalCount(liveNeighbors)));

    // A count of nine is more neighbours than a cell has.
    [Fact]
    public void IsBirthCount_MalformedRulestring_Throws()
    {
        var thrown = Assert.Throws<TypeInitializationException>(() => LifeLike<NineNeighbours>.IsBirthCount(3));

        Assert.IsType<InvalidOperationException>(thrown.InnerException);
    }

    private readonly struct Seeds : ILifeLikeRule
    {
        public static string Rulestring => "B2/S";
    }

    private readonly struct NineNeighbours : ILifeLikeRule
    {
        public static string Rulestring => "B39/S23";
    }
}
