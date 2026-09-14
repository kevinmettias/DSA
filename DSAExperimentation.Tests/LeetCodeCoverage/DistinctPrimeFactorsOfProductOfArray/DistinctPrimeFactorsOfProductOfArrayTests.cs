using DSAExperimentation.LeetCode.DistinctPrimeFactorsOfProductOfArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctPrimeFactorsOfProductOfArray;

// Harness only. Both strategies are DistinctPrimeFactorsOfProductOfArraySolution's
// - the BigInteger product the benchmark's baseline arm used to own and nothing
// asserted, and the per-element Set<int> union this file used to inline.
public sealed class DistinctPrimeFactorsOfProductOfArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LC examples 1-2.
            { [2, 4, 3, 7, 10, 6], 4 },
            { [2, 4, 8, 16], 1 },

            // A single element that is itself prime, and the largest one LC allows.
            { [997], 1 },
            { [2], 1 },

            // A single composite: 1000 = 2^3 * 5^3, so repeated factors collapse.
            { [1000], 2 },

            // Every element prime and distinct: nothing to dedup.
            { [2, 3, 5, 7, 11, 13], 6 },

            // Overlapping factor sets - 6, 10 and 15 pairwise share a prime, so the
            // union is smaller than the sum of the parts.
            { [6, 10, 15], 3 },

            // Prime powers: 512 = 2^9, 729 = 3^6, 625 = 5^4.
            { [512, 729, 625], 3 },

            // The same element repeated adds nothing after the first occurrence.
            { [12, 12, 12, 12], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistinctPrimeFactorsByProductTrialDivision_LeetCodeExamples_ReturnsDistinctPrimeCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByProductTrialDivision(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistinctPrimeFactorsByElementFactorSet_LeetCodeExamples_ReturnsDistinctPrimeCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByElementFactorSet(nums));
}
