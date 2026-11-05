using DSAExperimentation.LeetCode.Solutions.OpenTheLock;

namespace DSAExperimentation.LeetCode.Solutions.Tests.OpenTheLock;

public sealed partial class OpenTheLockReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(OpenTheLockReach.IsWithin(new string[3], new string('a', 0)));

    [Fact]
    public void IsWithin_DeadendsLengthPastTheReach_IsFalse() => Assert.False(OpenTheLockReach.IsWithin(new string[4], new string('a', 0)));
}
