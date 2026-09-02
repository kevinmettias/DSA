using DSAExperimentation.LeetCode.SuperPow;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SuperPow;

// Harness only: both strategies live in SuperPowSolution and are asserted against the
// same examples.
public sealed class SuperPowTests
{
    public static TheoryData<int, int[], int> Examples =>
        new()
        {
            { 2, [3], 8 },
            { 2, [1, 0], 1024 },
            { 1, [4, 3, 3, 8, 5, 2], 1 },
            { 3, [1, 0], 221 }, // 3^10 = 59049, 59049 mod 1337 = 221
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SuperPowByRepeatedMultiplication_LeetCodeExamples_ReturnsModularPower(int a, int[] b, int expected) =>
        Assert.Equal(expected, SuperPowSolution.SuperPowByRepeatedMultiplication(a, b));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SuperPowByHornerSquaring_LeetCodeExamples_ReturnsModularPower(int a, int[] b, int expected) =>
        Assert.Equal(expected, SuperPowSolution.SuperPowByHornerSquaring(a, b));
}
