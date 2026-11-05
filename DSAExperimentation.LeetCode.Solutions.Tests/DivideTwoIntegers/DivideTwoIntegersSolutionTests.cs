using DSAExperimentation.LeetCode.Solutions.DivideTwoIntegers;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DivideTwoIntegers;

// Harness only: both strategies live in DivideTwoIntegersSolution and are
// asserted against the same case: int.MaxValue divided by 7, a dividend at the top
// of the int32 range. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class DivideTwoIntegersSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { int.MaxValue, 7, 306_783_378 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DivideByBuiltInDivision_Cases_TruncatesTowardZero(
        int dividend, int divisor, int expected)
    {
        var actual = DivideTwoIntegersSolution.DivideByBuiltInDivision(dividend, divisor);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void DivideByBinarySearchProduct_Cases_TruncatesTowardZero(
        int dividend, int divisor, int expected)
    {
        var actual = DivideTwoIntegersSolution.DivideByBinarySearchProduct(dividend, divisor);
        Assert.Equal(expected, actual);
    }
}
