using DSAExperimentation.LeetCode.Solutions.DistinctPrimeFactorsOfProductOfArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DistinctPrimeFactorsOfProductOfArray;

// Harness only. Both strategies are DistinctPrimeFactorsOfProductOfArraySolution's
// - the BigInteger product the benchmark's baseline arm used to own and nothing
// asserted, and the per-element Set<int> union this file used to inline.
public sealed partial class DistinctPrimeFactorsOfProductOfArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // A single element that is itself prime, and the largest one LC allows.
            { [997], 1 },
            { [2], 1 },

            // A single composite: 1000 = 2^3 * 5^3, so repeated factors collapse.
            { [1000], 2 },

            // Overlapping factor sets - 6, 10 and 15 pairwise share a prime, so the
            // union is smaller than the sum of the parts.
            { [6, 10, 15], 3 },

            // Prime powers: 512 = 2^9, 729 = 3^6, 625 = 5^4.
            { [512, 729, 625], 3 },

            // The same element repeated adds nothing after the first occurrence.
            { [12, 12, 12, 12], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistinctPrimeFactorsByProductTrialDivision_Cases_ReturnsDistinctPrimeCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByProductTrialDivision(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void DistinctPrimeFactorsByElementFactorSet_Cases_ReturnsDistinctPrimeCount(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByElementFactorSet(nums));
}
