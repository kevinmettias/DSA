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
    public static TheoryData<UglyExample> Examples =>
        new()
        {
            // LC example 1.
            { new UglyExample(N: 3, A: 2, B: 3, C: 5, Expected: 4) },

            // LC example 2: 4 is a multiple of 2, so the third set adds nothing.
            { new UglyExample(N: 4, A: 2, B: 3, C: 4, Expected: 6) },

            // LC example 3.
            { new UglyExample(N: 5, A: 2, B: 11, C: 13, Expected: 10) },

            // The first requested number is always the smallest factor - and here
            // lcm(4, 6) already exceeds the search window, the case the capped
            // three-way LCM exists for.
            { new UglyExample(N: 1, A: 4, B: 6, C: 9, Expected: 4) },

            // All three factors equal: every LCM collapses onto the same set.
            { new UglyExample(N: 6, A: 3, B: 3, C: 3, Expected: 18) },

            // The (a, b, c) triple the benchmark measures.
            { new UglyExample(N: 10, A: 2, B: 3, C: 5, Expected: 14) },
        };

    // LeetCode's fourth published example. Its answer is close to 2e9, so only the
    // binary search is asserted against it - the counting walk is O(answer) and
    // would spend two billion steps arriving at the same number.
    public static TheoryData<UglyExample> LargeExamples =>
        new()
        {
            { new UglyExample(N: 1_000_000_000, A: 2, B: 217_983_653, C: 336_916_467, Expected: 1_999_999_984) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthUglyNumberByCountScan_LeetCodeExamples_ReturnsNthMultipleOfAnyFactor(UglyExample example)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByCountScan(example.N, example.A, example.B, example.C);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NthUglyNumberByBinarySearch_LeetCodeExamples_ReturnsNthMultipleOfAnyFactor(UglyExample example)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByBinarySearch(example.N, example.A, example.B, example.C);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void NthUglyNumberByBinarySearch_BillionthUglyNumber_CountsWithoutOverflowingTheLcm(UglyExample example)
    {
        var actual = UglyNumberIIISolution.NthUglyNumberByBinarySearch(example.N, example.A, example.B, example.C);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: how many multiples to count to, the three factors, and the
    // number that lands there. The five travel together at every row - three adjacent
    // ints and the count they are answered against - so they are one thing with a name
    // rather than five positional arguments a reader has to line up by hand.
    public readonly record struct UglyExample(int N, int A, int B, int C, int Expected);
}
