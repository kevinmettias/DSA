using DSAExperimentation.LeetCode.MinimizeTheMaximumOfTwoArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeTheMaximumOfTwoArrays;

// Harness only. Both strategies are MinimizeTheMaximumOfTwoArraysSolution's - the
// hand-rolled lo/hi bisection that used to live only in the benchmark's baseline
// arm, and the BinarySearch.LowerBound walk over the feasibility sequence the test
// used to inline.
public sealed class MinimizeTheMaximumOfTwoArraysTests
{
    public static TheoryData<int, int, int, int, int> Examples =>
        new()
        {
            // LC examples 1-3.
            { 2, 7, 1, 3, 4 },
            { 3, 5, 2, 1, 3 },
            { 2, 4, 8, 2, 15 },

            // Equal divisors: neither array may take an even number, so the two
            // arrays share the odd numbers 1 and 3.
            { 2, 2, 1, 1, 3 },

            // Equal divisors again, this time sparse enough that only the multiples
            // of 5 are excluded: 6 numbers are needed and [1, 7] supplies exactly 6.
            { 5, 5, 3, 3, 7 },

            // Coprime divisors, one number each: 1 is divisible by neither, so the
            // binding constraint is the pair needing two distinct numbers.
            { 2, 3, 1, 1, 2 },

            // Large enough that lcm = 6 inclusion-exclusion, not either divisor
            // alone, decides the answer.
            { 2, 3, 100, 100, 239 },

            // Lopsided counts: arr1 takes every odd number up to 9, and arr2's
            // single number comes from the evens 2, 4 and 8 that are left.
            { 2, 3, 5, 1, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizeSetByManualBisection_LeetCodeExamples_ReturnsSmallestFeasibleMaximum(
        int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2, int expected) =>
        Assert.Equal(
            expected,
            MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetByManualBisection(
                divisor1, divisor2, uniqueCnt1, uniqueCnt2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizeSetBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleMaximum(
        int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2, int expected) =>
        Assert.Equal(
            expected,
            MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetBySequenceLowerBound(
                divisor1, divisor2, uniqueCnt1, uniqueCnt2));
}
