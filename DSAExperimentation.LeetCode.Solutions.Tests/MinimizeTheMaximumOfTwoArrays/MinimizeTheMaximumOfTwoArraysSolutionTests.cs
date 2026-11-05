using DSAExperimentation.LeetCode.Solutions.MinimizeTheMaximumOfTwoArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimizeTheMaximumOfTwoArrays;

// Harness only. Both strategies are MinimizeTheMaximumOfTwoArraysSolution's - the
// hand-rolled lo/hi bisection that used to live only in the benchmark's baseline
// arm, and the predicate search over the candidate maximums that the test used to
// inline as a feasibility sequence.
public sealed partial class MinimizeTheMaximumOfTwoArraysSolutionTests
{
    public static TheoryData<MinimizeSetCase> Cases =>
        new()
        {
            // Equal divisors: neither array may take an even number, so the two
            // arrays share the odd numbers 1 and 3.
            { new MinimizeSetCase(Divisor1: 2, Divisor2: 2, UniqueCnt1: 1, UniqueCnt2: 1, Expected: 3) },

            // Equal divisors again, this time sparse enough that only the multiples
            // of 5 are excluded: 6 numbers are needed and [1, 7] supplies exactly 6.
            { new MinimizeSetCase(Divisor1: 5, Divisor2: 5, UniqueCnt1: 3, UniqueCnt2: 3, Expected: 7) },

            // Coprime divisors, one number each: 1 is divisible by neither, so the
            // binding constraint is the pair needing two distinct numbers.
            { new MinimizeSetCase(Divisor1: 2, Divisor2: 3, UniqueCnt1: 1, UniqueCnt2: 1, Expected: 2) },

            // Large enough that lcm = 6 inclusion-exclusion, not either divisor
            // alone, decides the answer.
            { new MinimizeSetCase(Divisor1: 2, Divisor2: 3, UniqueCnt1: 100, UniqueCnt2: 100, Expected: 239) },

            // Lopsided counts: arr1 takes every odd number up to 9, and arr2's
            // single number comes from the evens 2, 4 and 8 that are left.
            { new MinimizeSetCase(Divisor1: 2, Divisor2: 3, UniqueCnt1: 5, UniqueCnt2: 1, Expected: 9) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimizeSetByManualBisection_Cases_ReturnsSmallestFeasibleMaximum(
        MinimizeSetCase testCase)
    {
        var actual = MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetByManualBisection(
            testCase.Divisor1, testCase.Divisor2, testCase.UniqueCnt1, testCase.UniqueCnt2);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimizeSetByPredicateSearch_Cases_ReturnsSmallestFeasibleMaximum(
        MinimizeSetCase testCase)
    {
        var actual = MinimizeTheMaximumOfTwoArraysSolution.MinimizeSetByPredicateSearch(
            testCase.Divisor1, testCase.Divisor2, testCase.UniqueCnt1, testCase.UniqueCnt2);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case as one argument: the five values that describe a single row. They
    // travel together - a row IS one case - and passed separately they made a
    // five-parameter signature that could only be read by counting commas.
    public readonly record struct MinimizeSetCase(
        int Divisor1, int Divisor2, int UniqueCnt1, int UniqueCnt2, int Expected);
}
