using DSAExperimentation.LeetCode.Solutions.VowelsGameInAString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.VowelsGameInAString;

public sealed partial class VowelsGameInAStringReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(VowelsGameInAStringReach.IsWithin(new string('a', 36)));

    [Fact]
    public void IsWithin_TextLengthPastTheReach_IsFalse() => Assert.False(VowelsGameInAStringReach.IsWithin(new string('a', 37)));
}
