using DSAExperimentation.LeetCode.Solutions.SubsequencesWithAUniqueMiddleModeI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubsequencesWithAUniqueMiddleModeI;

public sealed partial class SubsequencesWithAUniqueMiddleModeIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SubsequencesWithAUniqueMiddleModeIReach.IsWithin(new int[28]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(SubsequencesWithAUniqueMiddleModeIReach.IsWithin(new int[29]));
}
