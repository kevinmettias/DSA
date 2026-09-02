using DSAExperimentation.LeetCode.PalindromePartitioning;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioning;

// Harness only. PalindromePartitioningSolution owns the backtracking walk; this
// file pins it to LeetCode's published examples.
public sealed class PalindromePartitioningTests
{
    public static TheoryData<string, string[][]> Examples =>
        new()
        {
            { "aab", [["a", "a", "b"], ["aa", "b"]] },
            { "a", [["a"]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByBacktracking_LeetCodeExamples_ReturnsEveryPalindromePartition(
        string s, string[][] expected) =>
        AssertSamePartitions(expected, PalindromePartitioningSolution.PartitionByBacktracking(s));

    private static void AssertSamePartitions(string[][] expected, List<List<string>> actual)
    {
        var actualArrays = actual.Select(x => x.ToArray()).ToArray();
        Assert.Equal(expected.Length, actualArrays.Length);

        foreach (var partition in expected)
        {
            Assert.Contains(actualArrays, x => x.SequenceEqual(partition));
        }
    }
}
