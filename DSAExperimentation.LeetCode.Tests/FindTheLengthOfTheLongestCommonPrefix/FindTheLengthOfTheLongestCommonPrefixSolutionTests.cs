using DSAExperimentation.LeetCode.FindTheLengthOfTheLongestCommonPrefix;

namespace DSAExperimentation.LeetCode.Tests.FindTheLengthOfTheLongestCommonPrefix;

// Harness only: both strategies live in
// FindTheLengthOfTheLongestCommonPrefixSolution - this file pins them to LeetCode's
// published examples, and the digit trie the trie strategy is handed to the keys and
// prefixes it should hold.
public sealed partial class FindTheLengthOfTheLongestCommonPrefixSolutionTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 10, 100], [1000], 3 },
            { [1, 2, 3], [4, 4, 4], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixLengthByBruteForce_LeetCodeExamples_ReturnsLongestSharedDigitPrefix(
        int[] arr1, int[] arr2, int expected)
    {
        var actual =
            FindTheLengthOfTheLongestCommonPrefixSolution.LongestPrefixLengthByBruteForce(arr1, arr2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestPrefixLengthByTrie_LeetCodeExamples_ReturnsLongestSharedDigitPrefix(
        int[] arr1, int[] arr2, int expected)
    {
        var actual = FindTheLengthOfTheLongestCommonPrefixSolution.LongestPrefixLengthByTrie(arr1, arr2);

        Assert.Equal(expected, actual);
    }

    // LeetCode's first example: 1, 10 and 100 go in as the digit strings "1", "10" and
    // "100", one key each. arr2's 1000 can follow them for three digits and no further,
    // and nothing in arr1 starts with 0.
    [Fact]
    public void BuildDigitTrie_LeetCodeFirstExample_HoldsEachValueAsItsDigitString()
    {
        var trie = FindTheLengthOfTheLongestCommonPrefixSolution.BuildDigitTrie([1, 10, 100]);
        var keys = new[] { "1", "10", "100", "1000" }.Select(trie.HasKey);
        var prefixes = new[] { "100", "1000", "0" }.Select(trie.HasPrefix);

        Assert.Equal(3, trie.Count);
        Assert.Equal([true, true, true, false], keys);
        Assert.Equal([true, false, false], prefixes);
    }
}
