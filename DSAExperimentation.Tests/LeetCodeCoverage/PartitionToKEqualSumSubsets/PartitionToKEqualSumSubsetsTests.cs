using DSAExperimentation.LeetCode.PartitionToKEqualSumSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionToKEqualSumSubsets;

// Harness only. Both strategies are PartitionToKEqualSumSubsetsSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class PartitionToKEqualSumSubsetsTests
{
    public static TheoryData<KSubsetExample> Examples =>
        new()
        {
            { new KSubsetExample(Nums: [4, 3, 2, 3, 5, 2, 1], K: 4, Expected: true) },
            { new KSubsetExample(Nums: [1, 2, 3, 4], K: 3, Expected: false) },
            { new KSubsetExample(Nums: [2, 2, 2, 2, 3, 4, 5], K: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionKSubsetsByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherKEqualBucketsExist(
        KSubsetExample example)
    {
        var actual = PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByNaiveBacktracking(
            example.Nums, example.K);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanPartitionKSubsetsByGenericBacktrack_LeetCodeExamples_ReturnsWhetherKEqualBucketsExist(
        KSubsetExample example)
    {
        var actual = PartitionToKEqualSumSubsetsSolution.CanPartitionKSubsetsByGenericBacktrack(
            example.Nums, example.K);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the numbers, how many equal-sum buckets they have to fill,
    // and whether that is possible. The answer is the datum under test, so the row
    // names it rather than leaving a bare `bool` after the bucket count.
    public readonly record struct KSubsetExample(int[] Nums, int K, bool Expected);
}
