using DSAExperimentation.LeetCode.Solutions.PowXn;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PowXn;

public sealed partial class PowXnReachTests
{
    public static TheoryData<int, bool> Powers =>
        new()
        {
            { 1_000_001, true },
            { -1_000_001, true },
            { 1_000_002, false },
            { -1_000_002, false },
            { int.MinValue, false },
        };

    [Theory]
    [MemberData(nameof(Powers))]
    public void IsWithin_Power_HoldsWhileItsMagnitudeIsWithinTheReach(int power, bool expected) =>
        Assert.Equal(expected, PowXnReach.IsWithin(2.0, power));
}
