using DSAExperimentation.LeetCode.Solutions.CountSubtreesWithMaxDistanceBetweenCities;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSubtreesWithMaxDistanceBetweenCities;

public sealed partial class CountSubtreesWithMaxDistanceBetweenCitiesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountSubtreesWithMaxDistanceBetweenCitiesReach.IsWithin(14, Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));

    [Fact]
    public void IsWithin_CityCountPastTheReach_IsFalse() => Assert.False(CountSubtreesWithMaxDistanceBetweenCitiesReach.IsWithin(15, Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));
}
