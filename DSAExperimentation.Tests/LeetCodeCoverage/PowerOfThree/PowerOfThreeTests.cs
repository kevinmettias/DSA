using DSAExperimentation.LeetCode.PowerOfThree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfThree;

// Harness only: both strategies live in PowerOfThreeSolution and are asserted
// against the same examples.
public sealed partial class PowerOfThreeTests
{
    public static TheoryData<PowerOfThreeCase> Examples =>
        new()
        {
            { new PowerOfThreeCase(N: 1, Expected: true) },
            { new PowerOfThreeCase(N: 3, Expected: true) },
            { new PowerOfThreeCase(N: 9, Expected: true) },
            { new PowerOfThreeCase(N: 27, Expected: true) },
            { new PowerOfThreeCase(N: 45, Expected: false) },
            { new PowerOfThreeCase(N: 0, Expected: false) },
            { new PowerOfThreeCase(N: -3, Expected: false) },
            // 3^19, the largest power of three that fits in an int.
            { new PowerOfThreeCase(N: 1162261467, Expected: true) },
            { new PowerOfThreeCase(N: 1162261466, Expected: false) },
            { new PowerOfThreeCase(N: int.MaxValue, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfThreeByDivisionLoop_LeetCodeExamples_ReturnsExpected(PowerOfThreeCase example) =>
        Assert.Equal(example.Expected, PowerOfThreeSolution.IsPowerOfThreeByDivisionLoop(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsPowerOfThreeByBinarySearch_LeetCodeExamples_ReturnsExpected(PowerOfThreeCase example) =>
        Assert.Equal(example.Expected, PowerOfThreeSolution.IsPowerOfThreeByBinarySearch(example.N));

    // One LeetCode example: the candidate integer and whether it is a power of
    // three. The expected value is named at every construction site, so a row reads
    // as the case it is rather than as a bare `true` whose meaning is its position.
    // Nested because it is only ever used inside this test class: it is this
    // harness's own vocabulary, not a type another file would import.
    public readonly record struct PowerOfThreeCase(int N, bool Expected);
}
