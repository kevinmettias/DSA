using DSAExperimentation.LeetCode.MaximizeTheNumberOfPartitionsAfterOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeTheNumberOfPartitionsAfterOperations;

// Harness only: the algorithms live in
// MaximizeTheNumberOfPartitionsAfterOperationsSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke (TwoSumTests precedent).
public sealed class MaximizeTheNumberOfPartitionsAfterOperationsTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "accca", 2, 3 },
            { "aabaab", 3, 1 },
            { "xxyz", 1, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionsByBruteForceRecolor_LeetCodeExamples_ReturnsMostPartitions(
        string text, int distinctLimit, int expected)
    {
        var actual = MaximizeTheNumberOfPartitionsAfterOperationsSolution.MaxPartitionsByBruteForceRecolor(text, distinctLimit);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPartitionsByBitmaskMemo_LeetCodeExamples_ReturnsMostPartitions(
        string text, int distinctLimit, int expected)
    {
        var actual = MaximizeTheNumberOfPartitionsAfterOperationsSolution.MaxPartitionsByBitmaskMemo(text, distinctLimit);
        Assert.Equal(expected, actual);
    }
}
