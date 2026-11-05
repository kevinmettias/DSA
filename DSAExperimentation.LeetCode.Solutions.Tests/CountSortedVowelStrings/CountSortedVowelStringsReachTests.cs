using DSAExperimentation.LeetCode.Solutions.CountSortedVowelStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSortedVowelStrings;

public sealed partial class CountSortedVowelStringsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountSortedVowelStringsReach.IsWithin(49));

    [Fact]
    public void IsWithin_LengthPastTheReach_IsFalse() => Assert.False(CountSortedVowelStringsReach.IsWithin(50));
}
