using DSAExperimentation.LeetCode.FindTheLengthOfTheLongestCommonPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheLengthOfTheLongestCommonPrefix;

// Harness only: both strategies live in
// FindTheLengthOfTheLongestCommonPrefixSolution - this file just pins them to
// LeetCode's published examples.
public sealed class FindTheLengthOfTheLongestCommonPrefixTests
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
}
