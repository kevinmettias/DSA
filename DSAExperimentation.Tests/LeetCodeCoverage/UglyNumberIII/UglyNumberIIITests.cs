using DSAExperimentation.LeetCode.UglyNumberIII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UglyNumberIII;

// Harness only: both strategies live in UglyNumberIIISolution and are asserted
// against the same examples - LeetCode's first three published ones, the case
// where one factor divides another (inclusion-exclusion subtracts the whole
// overlap), the case where all three coincide, the first-element case where the
// answer is just the smallest factor, and the (a, b, c) triple the benchmark
// measures.
public sealed class UglyNumberIIITests
{
    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            // LC example 1.
            { 3, 2, 3, 5, 4 },

            // LC example 2: 4 is a multiple of 2, so the third set adds nothing.
            { 4, 2, 3, 4, 6 },

            // LC example 3.
            { 5, 2, 11, 13, 10 },

            // The first requested number is always the smallest factor - and here
            // lcm(4, 6) already exceeds the search window, the case the capped
            // three-way LCM exists for.
            { 1, 4, 6, 9, 4 },

            // All three factors equal: every LCM collapses onto the same set.
            { 6, 3, 3, 3, 18 },

            // The (a, b, c) triple the benchmark measures.
            { 10, 2, 3, 5, 14 },
        };

    // LeetCode's fourth published example. Its answer is close to 2e9, so only the
    // binary search is asserted against it - the counting walk is O(answer) and
    // would spend two billion steps arriving at the same number.
    public static TheoryData<int, int, int, int, int> LargeExamples =>
        new()
        {
            { 1_000_000_000, 2, 217_983_653, 336_916_467, 1_999_999_984 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthUglyNumberByCountScan_LeetCodeExamples_ReturnsNthMultipleOfAnyFactor(
        int n, int a, int b, int c, int expected) =>
        Assert.Equal(expected, UglyNumberIIISolution.NthUglyNumberByCountScan(n, a, b, c));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthUglyNumberByBinarySearch_LeetCodeExamples_ReturnsNthMultipleOfAnyFactor(
        int n, int a, int b, int c, int expected) =>
        Assert.Equal(expected, UglyNumberIIISolution.NthUglyNumberByBinarySearch(n, a, b, c));

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void NthUglyNumberByBinarySearch_BillionthUglyNumber_CountsWithoutOverflowingTheLcm(
        int n, int a, int b, int c, int expected) =>
        Assert.Equal(expected, UglyNumberIIISolution.NthUglyNumberByBinarySearch(n, a, b, c));
}
