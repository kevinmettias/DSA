using DSAExperimentation.LeetCode.SplitTheArrayToMakeCoprimeProducts;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SplitTheArrayToMakeCoprimeProducts;

// Harness only. Both strategies are SplitTheArrayToMakeCoprimeProductsSolution's -
// the BigInteger gcd scan the benchmark's baseline arm used to own and nothing
// asserted, and the HashMap<prime,lastIndex> boundary sweep this file used to
// inline.
public sealed partial class SplitTheArrayToMakeCoprimeProductsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            // LC examples 1-2.
            { [2, 3, 3], 0 },
            { [4, 6, 8], -1 },

            // The shortest possible input, split at its only candidate index.
            { [2, 7], 0 },
            { [4, 6], -1 },

            // The split is neither the first nor the last candidate: 6*2*3 = 36 and
            // 35*11 = 385 are coprime, but the two earlier splits are not.
            { [6, 2, 3, 35, 11], 2 },

            // Ones carry no prime factors at all, so the empty left product 1 is
            // coprime to everything and index 0 always answers.
            { [1, 1, 1], 0 },
            { [1, 2, 1, 2], 0 },
            { [2, 1, 3], 0 },

            // Only the last candidate index works: the 2s all sit left of the 9.
            { [4, 4, 4, 9], 2 },

            // A prime reappearing at the very end blocks every split before it.
            { [2, 3, 5, 2], -1 },

            // Composite values sharing several primes: 12 and 18 both carry 2 and 3,
            // so the first split fails and the second succeeds against 25*35.
            { [12, 18, 25, 35], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindValidSplitByProductGcd_LeetCodeExamples_ReturnsSmallestCoprimeSplitIndex(
        int[] nums, int expected) =>
        Assert.Equal(expected, SplitTheArrayToMakeCoprimeProductsSolution.FindValidSplitByProductGcd(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindValidSplitByPrimeLastOccurrence_LeetCodeExamples_ReturnsSmallestCoprimeSplitIndex(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            SplitTheArrayToMakeCoprimeProductsSolution.FindValidSplitByPrimeLastOccurrence(nums));
}
