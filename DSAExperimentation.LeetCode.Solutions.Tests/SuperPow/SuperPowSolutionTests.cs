using DSAExperimentation.LeetCode.Solutions.SuperPow;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SuperPow;

// Harness only: both strategies live in SuperPowSolution and are asserted against the
// same case, 3 raised to a two-digit exponent. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class SuperPowSolutionTests
{
    public static TheoryData<int, int[], int> Cases =>
        new()
        {
            { 3, [1, 0], 221 }, // 3^10 = 59049, 59049 mod 1337 = 221
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SuperPowByRepeatedMultiplication_Cases_ReturnsModularPower(
        int baseValue, int[] exponentDigits, int expected)
    {
        var actual = SuperPowSolution.SuperPowByRepeatedMultiplication(baseValue, exponentDigits);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SuperPowByHornerSquaring_Cases_ReturnsModularPower(
        int baseValue, int[] exponentDigits, int expected)
    {
        var actual = SuperPowSolution.SuperPowByHornerSquaring(baseValue, exponentDigits);

        Assert.Equal(expected, actual);
    }
}
