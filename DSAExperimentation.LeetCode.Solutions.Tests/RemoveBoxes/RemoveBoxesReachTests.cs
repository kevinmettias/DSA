using DSAExperimentation.LeetCode.Solutions.RemoveBoxes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveBoxes;

public sealed partial class RemoveBoxesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(RemoveBoxesReach.IsWithin(new int[17]));

    [Fact]
    public void IsWithin_BoxesLengthPastTheReach_IsFalse() => Assert.False(RemoveBoxesReach.IsWithin(new int[18]));
}
