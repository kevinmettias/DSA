using DSAExperimentation.LeetCode.Solutions.CyclicallyRotatingAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CyclicallyRotatingAGrid;

public sealed partial class CyclicallyRotatingAGridReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CyclicallyRotatingAGridReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 5).ToArray(), 50));

    [Fact]
    public void IsWithin_GridLengthPastTheReach_IsFalse() => Assert.False(CyclicallyRotatingAGridReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 6).ToArray(), 50));

    [Fact]
    public void IsWithin_RotationStepsPastTheReach_IsFalse() => Assert.False(CyclicallyRotatingAGridReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 5).ToArray(), 51));
}
