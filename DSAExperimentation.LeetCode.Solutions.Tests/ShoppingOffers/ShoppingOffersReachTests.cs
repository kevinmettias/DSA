using DSAExperimentation.LeetCode.Solutions.ShoppingOffers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShoppingOffers;

public sealed partial class ShoppingOffersReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ShoppingOffersReach.IsWithin(new int[0], Enumerable.Repeat(Array.Empty<int>(), 5).ToArray(), new int[0]));

    [Fact]
    public void IsWithin_SpecialLengthPastTheReach_IsFalse() => Assert.False(ShoppingOffersReach.IsWithin(new int[0], Enumerable.Repeat(Array.Empty<int>(), 6).ToArray(), new int[0]));
}
