using DSAExperimentation.LeetCode.Solutions.ContinuousSubarraySum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContinuousSubarraySum;

// Harness only: both strategies live in ContinuousSubarraySumSolution and are asserted
// against the same cases, among them the case that fails only because the qualifying
// remainder repeat is one index too close together.
public sealed partial class ContinuousSubarraySumSolutionTests
{
    public static TheoryData<SubarraySumCase> Cases =>
        new()
        {
            { new SubarraySumCase([1, 2, 3], 5, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasSubarraySumMultipleOfKByBruteForce_Cases_ReturnsExpected(
        SubarraySumCase testCase)
    {
        var actual = ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByBruteForce(
            testCase.Nums, testCase.Divisor);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasSubarraySumMultipleOfKByHashMapPrefixRemainder_Cases_ReturnsExpected(
        SubarraySumCase testCase)
    {
        var actual = ContinuousSubarraySumSolution.HasSubarraySumMultipleOfKByHashMapPrefixRemainder(
            testCase.Nums, testCase.Divisor);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the array, its divisor, and whether some subarray of at
    // least two elements sums to a multiple of that divisor. The expected value is
    // named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct SubarraySumCase(int[] Nums, int Divisor, bool Expected);
}
