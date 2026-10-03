using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

public sealed partial class IntegerSquareRootTests
{
    private const int CheckedUpTo = 10_000;

    public static TheoryData<int, int> ValuesToExpectedRoots =>
        new() { { 0, 0 }, { 1, 1 }, { 3, 1 }, { 4, 2 }, { 8, 2 }, { 9, 3 }, { 2_147_395_600, 46_340 } };

    [Theory]
    [MemberData(nameof(ValuesToExpectedRoots))]
    public void Floor_Values_ReturnsTheFloorOfTheSquareRoot(int value, int expected) =>
        Assert.Equal(expected, IntegerSquareRoot.Floor(value));

    // floor(sqrt(n)) is the r with r^2 <= n < (r + 1)^2 - checked directly, not against Math.Sqrt.
    [Fact]
    public void Floor_EveryValueUpToABound_BracketsTheValue()
    {
        for (var value = 0; value <= CheckedUpTo; value++)
        {
            var root = (long)IntegerSquareRoot.Floor(value);

            Assert.InRange((long)value, root * root, ((root + 1) * (root + 1)) - 1);
        }
    }

    // The search is capped at 46,341 so squaring an index stays in long; the cap must still admit
    // int.MaxValue's own root.
    [Fact]
    public void Floor_IntMaxValue_ReturnsItsRootUnderTheCap() => Assert.Equal(46_340, IntegerSquareRoot.Floor(int.MaxValue));

    [Fact]
    public void Floor_NegativeValue_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => IntegerSquareRoot.Floor(-1));
}
