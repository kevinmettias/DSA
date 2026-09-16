using DSAExperimentation.LeetCode.PowerOfFour;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfFour;

// Harness only: both strategies live in PowerOfFourSolution and are asserted
// against the same examples.
public sealed partial class PowerOfFourTests
{
    public static TheoryData<PowerOfFourCase> Examples =>
        new()
        {
            { new PowerOfFourCase(N: 1, Expected: true) },
            { new PowerOfFourCase(N: 16, Expected: true) },
            { new PowerOfFourCase(N: 5, Expected: false) },
            { new PowerOfFourCase(N: 0, Expected: false) },
            { new PowerOfFourCase(N: -4, Expected: false) },
            // 4^15, the largest power of four an int holds.
            { new PowerOfFourCase(N: 1073741824, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfFourByDivisionLoop_LeetCodeExamples_ReturnsExpected(PowerOfFourCase example) =>
        Assert.Equal(example.Expected, PowerOfFourSolution.IsPowerOfFourByDivisionLoop(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfFourByBinarySearch_LeetCodeExamples_ReturnsExpected(PowerOfFourCase example) =>
        Assert.Equal(example.Expected, PowerOfFourSolution.IsPowerOfFourByBinarySearch(example.N));

    // One LeetCode example: the candidate integer and whether it is a power of
    // four. The expected value is named at every construction site, so a row reads
    // as the case it is rather than as a bare `true` whose meaning is its position.
    // Nested because it is only ever used inside this test class: it is this
    // harness's own vocabulary, not a type another file would import.
    public readonly record struct PowerOfFourCase(int N, bool Expected);
}
