using DSAExperimentation.LeetCode.PalindromePartitioning;

namespace DSAExperimentation.LeetCode.Tests.PalindromePartitioning;

// Harness only. Both strategies live in PalindromePartitioningSolution - the
// backtracking walk and the precomputed-table walk - so this file pins them to
// LeetCode's published examples and to each other. Partition order is not part of
// LeetCode's contract, so each result is checked as a set of partitions.
public sealed partial class PalindromePartitioningSolutionTests
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
        string text, string[][] expected) =>
        AssertSamePartitions(expected, PalindromePartitioningSolution.PartitionByBacktracking(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PartitionByPrecomputedPalindromeTable_LeetCodeExamples_ReturnsEveryPalindromePartition(
        string text, string[][] expected) =>
        AssertSamePartitions(expected, PalindromePartitioningSolution.PartitionByPrecomputedPalindromeTable(text));

    private static void AssertSamePartitionsBetween(List<List<string>> first, List<List<string>> second)
    {
        var firstArrays = first.Select(x => x.ToArray()).ToArray();
        var secondArrays = second.Select(x => x.ToArray()).ToArray();

        Assert.Equal(firstArrays.Length, secondArrays.Length);

        foreach (var partition in firstArrays)
        {
            Assert.Contains(secondArrays, x => x.SequenceEqual(partition));
        }
    }

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
