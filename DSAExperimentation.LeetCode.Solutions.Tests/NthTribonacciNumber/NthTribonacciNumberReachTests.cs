using DSAExperimentation.LeetCode.Solutions.NthTribonacciNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NthTribonacciNumber;

public sealed partial class NthTribonacciNumberReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NthTribonacciNumberReach.IsWithin(30));

    [Fact]
    public void IsWithin_TermIndexPastTheReach_IsFalse() => Assert.False(NthTribonacciNumberReach.IsWithin(31));
}
