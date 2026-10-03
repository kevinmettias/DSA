using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class LeastCommonMultipleTests
{
    public static TheoryData<int, int, int> IntCasesToExpectedMultiples =>
        new() { { 4, 6, 12 }, { 6, 4, 12 }, { 7, 13, 91 }, { 5, 5, 5 }, { 1, 9, 9 }, { 0, 9, 0 }, { 9, 0, 0 } };

    [Theory]
    [MemberData(nameof(IntCasesToExpectedMultiples))]
    public void Of_IntPairs_ReturnsTheLeastCommonMultiple(int first, int second, int expected) =>
        Assert.Equal(expected, LeastCommonMultiple.Of(first, second));

    [Fact]
    public void Of_NegativeInput_ReturnsThePositiveMultiple() => Assert.Equal(12, LeastCommonMultiple.Of(-4, 6));

    // Widening first is the caller's job: two ints whose lcm passes int.MaxValue fit once taken as long.
    [Fact]
    public void Of_WidenedToLong_HoldsAnLcmPastIntRange() =>
        Assert.Equal(2_147_395_600L * 3, LeastCommonMultiple.Of(2_147_395_600L, 3L));
}
