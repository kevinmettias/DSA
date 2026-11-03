using DSAExperimentation.LeetCode.Solutions.FindTheLengthOfTheLongestCommonPrefix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheLengthOfTheLongestCommonPrefix;

// Harness only: both strategies live in FindTheLengthOfTheLongestCommonPrefixSolution, and
// CatalogCaseTests holds them to LeetCode's published examples (ARCHITECTURE 17.13). This
// file pins the digit trie the trie strategy is handed to the keys and prefixes it should
// hold.
public sealed partial class FindTheLengthOfTheLongestCommonPrefixSolutionTests
{
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
