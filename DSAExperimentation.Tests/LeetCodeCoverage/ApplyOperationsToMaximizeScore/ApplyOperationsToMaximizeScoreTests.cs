using DSAExperimentation.LeetCode.ApplyOperationsToMaximizeScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ApplyOperationsToMaximizeScore;

// Harness only. Both strategies are ApplyOperationsToMaximizeScoreSolution's -
// including the outward boundary scan, which the benchmark used to own privately as
// its baseline with nothing asserting it. Beyond LeetCode's two published examples the
// cases pin what the boundary arithmetic and the greedy have to get right: a single
// element, values whose prime score is 0 (every nums[i] = 1), score ties that force
// the leftmost-owner rule, k exactly equal to the subarray count, k larger than it,
// and a product big enough to wrap the 1e9+7 modulus. Every expectation was computed
// from the problem statement directly - enumerate all subarrays, take each one's
// picked element, multiply the k largest - not from either strategy.
public sealed class ApplyOperationsToMaximizeScoreTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            // LC example 1: every element has prime score 1, so the two 8s win.
            { [8, 3, 9, 3, 8], 2, 81L },

            // LC example 2.
            { [19, 12, 14, 6, 10, 18], 3, 4788L },

            // One element, one subarray, one operation.
            { [2], 1, 2L },

            // Tied scores: the leftmost owner rule gives index 0 two subarrays.
            { [2, 3], 3, 12L },

            // Equal values as well as equal scores - the tie ordering cannot matter.
            { [4, 4], 2, 16L },

            // 99991 * 99991 * 99989 wraps the modulus twice over.
            { [99991, 99989], 3, 20901139L },

            // Prime score 0 everywhere: the answer is 1 however many operations run.
            { [1, 1, 1], 6, 1L },

            // k equals the subarray count, so every operation is spent.
            { [2, 4, 8, 16], 10, 1048576L },

            // k exceeds the subarray count - the greedy simply runs out of subarrays.
            { [3, 5], 5, 45L },

            // Four equal values: only three of the ten subarrays are ever needed.
            { [7, 7, 7, 7], 3, 343L },

            // Distinct scores 2, 3, 1, 1 - the highest-score index owns the most range.
            { [6, 30, 2, 3], 5, 24300000L },

            // A high-score index at each end, so the right one's range stops at the left.
            { [12, 7, 9, 2, 15], 4, 50625L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByLinearBoundaryScan_LeetCodeExamples_ReturnsGreedyProductModulo(
        int[] nums, int k, long expected) =>
        Assert.Equal(expected, ApplyOperationsToMaximizeScoreSolution.MaximumScoreByLinearBoundaryScan(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByStackBoundaryScan_LeetCodeExamples_ReturnsGreedyProductModulo(
        int[] nums, int k, long expected) =>
        Assert.Equal(expected, ApplyOperationsToMaximizeScoreSolution.MaximumScoreByStackBoundaryScan(nums, k));
}
