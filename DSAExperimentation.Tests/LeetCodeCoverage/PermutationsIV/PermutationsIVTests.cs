using DSAExperimentation.LeetCode.PermutationsIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationsIV;

// Harness only. The factorial-number-system unranking for both strategies is
// PermutationsIVSolution's - this file just pins them to LeetCode's published
// examples, including the k-too-large case that must come back empty.
public sealed class PermutationsIVTests
{
    public static TheoryData<int, long, int[]> Examples =>
        new()
        {
            { 4, 6, [3, 4, 1, 2] },
            { 3, 2, [3, 2, 1] },
            { 2, 3, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthPermutationByBigIntegerRank_LeetCodeExamples_ReturnsKthAlternatingPermutation(
        int permutationLength, long targetRank, int[] expected)
    {
        var actual = PermutationsIVSolution.KthPermutationByBigIntegerRank(permutationLength, targetRank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthPermutationByFenwickOrderStatistics_LeetCodeExamples_ReturnsKthAlternatingPermutation(
        int permutationLength, long targetRank, int[] expected)
    {
        var actual = PermutationsIVSolution.KthPermutationByFenwickOrderStatistics(permutationLength, targetRank);

        Assert.Equal(expected, actual);
    }
}
