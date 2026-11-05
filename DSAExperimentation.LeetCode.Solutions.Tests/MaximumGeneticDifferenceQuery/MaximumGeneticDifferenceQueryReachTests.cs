using DSAExperimentation.LeetCode.Solutions.MaximumGeneticDifferenceQuery;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumGeneticDifferenceQuery;

public sealed partial class MaximumGeneticDifferenceQueryReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MaximumGeneticDifferenceQueryReach.IsWithin(new int[0], Enumerable.Repeat(Array.Empty<int>(), 4).ToArray()));

    [Fact]
    public void IsWithin_QueriesLengthPastTheReach_IsFalse() => Assert.False(MaximumGeneticDifferenceQueryReach.IsWithin(new int[0], Enumerable.Repeat(Array.Empty<int>(), 5).ToArray()));
}
