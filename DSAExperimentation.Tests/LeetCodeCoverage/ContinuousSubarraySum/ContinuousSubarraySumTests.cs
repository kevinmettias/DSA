using DSAExperimentation.LeetCode.ContinuousSubarraySum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContinuousSubarraySum;

// Harness only: both strategies live in ContinuousSubarraySumSolution and are asserted
// against the same examples, including the case that fails only because the qualifying
// remainder repeat is one index too close together.
public sealed partial class ContinuousSubarraySumTests
{
    public static TheoryData<SubarraySumCase> Examples =>
        new()
        {
            { new SubarraySumCase([23, 2, 4, 6, 7], 6, Expected: true) },
            { new SubarraySumCase([23, 2, 6, 4, 7], 6, Expected: true) },
            { new SubarraySumCase([23, 2, 6, 4, 7], 13, Expected: false) },
            { new SubarraySumCase([1, 2, 3], 5, Expected: true) },
            { new SubarraySumCase([1, 2, 12], 6, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasSubarraySumMultipleOfKByBruteForce_LeetCodeExamples_ReturnsExpected(
        SubarraySumCase example)
    {
        var actual = ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByBruteForce(
            example.Nums, example.Divisor);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasSubarraySumMultipleOfKByHashMapPrefixRemainder_LeetCodeExamples_ReturnsExpected(
        SubarraySumCase example)
    {
        var actual = ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByHashMapPrefixRemainder(
            example.Nums, example.Divisor);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the array, its divisor, and whether some subarray of at
    // least two elements sums to a multiple of that divisor. The expected value is
    // named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct SubarraySumCase(int[] Nums, int Divisor, bool Expected);
}
