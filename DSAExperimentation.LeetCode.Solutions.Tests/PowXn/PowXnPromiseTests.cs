using DSAExperimentation.LeetCode.Solutions.PowXn;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PowXn;

public sealed partial class PowXnPromiseTests
{
    public static TheoryData<double, int, bool> Inputs =>
        new()
        {
            { 2.0, 10, true },
            { 2.1, 3, true },
            { 2.0, -2, true },
            { 10.0, 100, false },
            { 0.1, -10, false },
            { 0.0, -1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePowerIsDefinedAndWithinTenThousand(double baseValue, int power, bool expected)
        => Assert.Equal(expected, PowXnPromise.IsKeptBy(baseValue, power));
}
