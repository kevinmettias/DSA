using DSAExperimentation.LeetCode.Solutions.CycleLengthQueriesInATree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CycleLengthQueriesInATree;

public sealed partial class CycleLengthQueriesInATreeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CycleLengthQueriesInATreeReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 29).ToArray()));

    [Fact]
    public void IsWithin_QueriesLengthPastTheReach_IsFalse() => Assert.False(CycleLengthQueriesInATreeReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 30).ToArray()));
}
