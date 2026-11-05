using DSAExperimentation.LeetCode.Solutions.FindTheShortestSuperstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheShortestSuperstring;

public sealed partial class FindTheShortestSuperstringReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(FindTheShortestSuperstringReach.IsWithin(new string[9]));

    [Fact]
    public void IsWithin_WordCountPastTheReach_IsFalse() => Assert.False(FindTheShortestSuperstringReach.IsWithin(new string[10]));
}
