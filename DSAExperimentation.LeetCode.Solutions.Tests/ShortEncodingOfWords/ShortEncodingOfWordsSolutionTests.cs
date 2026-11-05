using DSAExperimentation.LeetCode.Solutions.ShortEncodingOfWords;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortEncodingOfWords;

// Harness only. All three strategies are ShortEncodingOfWordsSolution's - this file
// pins them to the repeated-word case, which is
// the one every "drop the suffixes" strategy has to deduplicate first or it charges
// the same word twice.
public sealed partial class ShortEncodingOfWordsSolutionTests
{
    public static TheoryData<string[], int> Cases =>
        new()
        {
            { ["me", "time"], 5 },
            { ["time", "atime", "btime"], 12 },
            { ["time", "time", "time", "time"], 5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumLengthByPairwiseSuffixScan_Cases_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthByPairwiseSuffixScan(words));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumLengthBySuffixEviction_Cases_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthBySuffixEviction(words));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumLengthByReversedTrieLeaves_Cases_ReturnsShortestReferenceLength(
        string[] words, int expected) =>
        Assert.Equal(expected, ShortEncodingOfWordsSolution.MinimumLengthByReversedTrieLeaves(words));
}
