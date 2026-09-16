using DSAExperimentation.LeetCode.MinimizeTheMaximumOfTwoArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeTheMaximumOfTwoArrays;

// Harness only. Both strategies are MinimizeTheMaximumOfTwoArraysSolution's - the
// hand-rolled lo/hi bisection that used to live only in the benchmark's baseline
// arm, and the BinarySearch.LowerBound walk over the feasibility sequence the test
// used to inline.
public sealed class MinimizeTheMaximumOfTwoArraysTests
{
    public static TheoryData<MinimizeSetExample> Examples =>
        new()
        {
            // LC examples 1-3.
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 7, UniqueCnt1: 1, UniqueCnt2: 3, Expected: 4) },
            { new MinimizeSetExample(Divisor1: 3, Divisor2: 5, UniqueCnt1: 2, UniqueCnt2: 1, Expected: 3) },
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 4, UniqueCnt1: 8, UniqueCnt2: 2, Expected: 15) },

            // Equal divisors: neither array may take an even number, so the two
            // arrays share the odd numbers 1 and 3.
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 2, UniqueCnt1: 1, UniqueCnt2: 1, Expected: 3) },

            // Equal divisors again, this time sparse enough that only the multiples
            // of 5 are excluded: 6 numbers are needed and [1, 7] supplies exactly 6.
            { new MinimizeSetExample(Divisor1: 5, Divisor2: 5, UniqueCnt1: 3, UniqueCnt2: 3, Expected: 7) },

            // Coprime divisors, one number each: 1 is divisible by neither, so the
            // binding constraint is the pair needing two distinct numbers.
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 3, UniqueCnt1: 1, UniqueCnt2: 1, Expected: 2) },

            // Large enough that lcm = 6 inclusion-exclusion, not either divisor
            // alone, decides the answer.
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 3, UniqueCnt1: 100, UniqueCnt2: 100, Expected: 239) },

            // Lopsided counts: arr1 takes every odd number up to 9, and arr2's
            // single number comes from the evens 2, 4 and 8 that are left.
            { new MinimizeSetExample(Divisor1: 2, Divisor2: 3, UniqueCnt1: 5, UniqueCnt2: 1, Expected: 9) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizeSetByManualBisection_LeetCodeExamples_ReturnsSmallestFeasibleMaximum(
        MinimizeSetExample example)
    {
        var actual = MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetByManualBisection(
            example.Divisor1, example.Divisor2, example.UniqueCnt1, example.UniqueCnt2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimizeSetBySequenceLowerBound_LeetCodeExamples_ReturnsSmallestFeasibleMaximum(
        MinimizeSetExample example)
    {
        var actual = MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetBySequenceLowerBound(
            example.Divisor1, example.Divisor2, example.UniqueCnt1, example.UniqueCnt2);

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument: the five values that describe a single case. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct MinimizeSetExample(
        int Divisor1, int Divisor2, int UniqueCnt1, int UniqueCnt2, int Expected);
}
