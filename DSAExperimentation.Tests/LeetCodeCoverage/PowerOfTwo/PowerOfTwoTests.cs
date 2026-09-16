using DSAExperimentation.LeetCode.PowerOfTwo;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfTwo;

// Harness only: both strategies live in PowerOfTwoSolution and are asserted
// against the same examples.
public sealed class PowerOfTwoTests
{
    public static TheoryData<PowerOfTwoCase> Examples =>
        new()
        {
            { new PowerOfTwoCase(N: 1, Expected: true) },
            { new PowerOfTwoCase(N: 16, Expected: true) },
            { new PowerOfTwoCase(N: 3, Expected: false) },
            { new PowerOfTwoCase(N: 0, Expected: false) },
            { new PowerOfTwoCase(N: -1, Expected: false) },
            // 2^30, the largest power of two an int can hold.
            { new PowerOfTwoCase(N: 1_073_741_824, Expected: true) },
            { new PowerOfTwoCase(N: int.MaxValue, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfTwoByRepeatedDivision_LeetCodeExamples_ReturnsExpected(PowerOfTwoCase example) =>
        Assert.Equal(example.Expected, PowerOfTwoSolution.IsPowerOfTwoByRepeatedDivision(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfTwoByBitTrick_LeetCodeExamples_ReturnsExpected(PowerOfTwoCase example) =>
        Assert.Equal(example.Expected, PowerOfTwoSolution.IsPowerOfTwoByBitTrick(example.N));

    // One LeetCode example: the candidate integer and whether it is a power of two.
    // The expected value is named at every construction site, so a row reads as the
    // case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class: it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct PowerOfTwoCase(int N, bool Expected);
}
