using DSAExperimentation.LeetCode.Solutions.CountKReducibleNumbersLessThanN;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountKReducibleNumbersLessThanN;

public sealed partial class CountKReducibleNumbersLessThanNReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountKReducibleNumbersLessThanNReach.IsWithin(new string('a', 8), 1));

    [Fact]
    public void IsWithin_BinaryDigitsLengthPastTheReach_IsFalse() => Assert.False(CountKReducibleNumbersLessThanNReach.IsWithin(new string('a', 9), 1));
}
