using DSAExperimentation.LeetCode.ShortEncodingOfWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortEncodingOfWords;

// Harness only. All three strategies are ShortEncodingOfWordsSolution's - this file
// pins them to LeetCode's published examples plus the repeated-word case, which is
// the one every "drop the suffixes" strategy has to deduplicate first or it charges
// the same word twice.
public sealed partial class ShortEncodingOfWordsTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["time", "me", "bell"], 10 },
            { ["t"], 2 },
            { ["me", "time"], 5 },
            { ["time", "atime", "btime"], 12 },
            { ["time", "time", "time", "time"], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumLengthByPairwiseSuffixScan_LeetCodeExamples_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthByPairwiseSuffixScan(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumLengthBySuffixEviction_LeetCodeExamples_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthBySuffixEviction(words));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumLengthByReversedTrieLeaves_LeetCodeExamples_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthByReversedTrieLeaves(words));
}
