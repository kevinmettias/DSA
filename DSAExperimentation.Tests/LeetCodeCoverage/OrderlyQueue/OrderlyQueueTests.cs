using DSAExperimentation.LeetCode.OrderlyQueue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OrderlyQueue;

// Harness only. Both strategies are OrderlyQueueSolution's - one example table,
// one theory per strategy, so a failure names the strategy that broke.
public sealed class OrderlyQueueTests
{
    public static TheoryData<string, int, string> Examples =>
        new()
        {
            // LeetCode example 1: k == 1, so only rotations are reachable.
            { "cba", 1, "acb" },
            // LeetCode example 2: k > 1 reaches every permutation, so s sorted.
            { "baaca", 3, "aaabc" },
            // A single character has one rotation and one permutation.
            { "z", 1, "z" },
            // k == 1 on a two-character string: "ba" -> "ab".
            { "ba", 1, "ab" },
            // The same string as example 1, now with k > 1: the full sort wins.
            { "cba", 3, "abc" },
            // k == 1 where the best rotation is neither first nor last.
            { "bca", 1, "abc" },
            // Repeated characters: every rotation is identical.
            { "aaa", 1, "aaa" },
            // k == 2 is already the "every permutation" case, not a rotation case.
            { "dcab", 2, "abcd" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringByBruteForceRotations_LeetCodeExamples_ReturnsSmallestReachableString(
        string s, int k, string expected)
    {
        var actual = OrderlyQueueSolution.SmallestStringByBruteForceRotations(s, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestStringBySuffixArray_LeetCodeExamples_ReturnsSmallestReachableString(
        string s, int k, string expected)
    {
        var actual = OrderlyQueueSolution.SmallestStringBySuffixArray(s, k);

        Assert.Equal(expected, actual);
    }
}
