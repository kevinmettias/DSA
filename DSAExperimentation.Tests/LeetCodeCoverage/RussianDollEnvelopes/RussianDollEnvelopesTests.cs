using DSAExperimentation.LeetCode.RussianDollEnvelopes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RussianDollEnvelopes;

// Harness only. Both strategies are RussianDollEnvelopesSolution's - this file just pins them to
// LeetCode's published examples, including the all-tied-widths case that cannot chain at all.
public sealed partial class RussianDollEnvelopesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[5, 4], [6, 4], [6, 7], [2, 3]], 3 },
            { [[1, 1], [1, 1], [1, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxEnvelopesBySortThenPatience_LeetCodeExamples_ReturnsLongestChain(
        int[][] envelopes, int expected) =>
        Assert.Equal(expected, RussianDollEnvelopesSolution.MaxEnvelopesBySortThenPatience(envelopes));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxEnvelopesByBruteForceDp_LeetCodeExamples_ReturnsLongestChain(
        int[][] envelopes, int expected) =>
        Assert.Equal(expected, RussianDollEnvelopesSolution.MaxEnvelopesByBruteForceDp(envelopes));
}
