using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Harness;

// These comparers decide whether ~1100 problems' answers are right, so what they
// REJECT matters more than what they accept: a comparer that says yes to
// everything makes the whole harness green and meaningless. Every test below is
// paired - one answer that must pass and one wrong answer that must not.
public sealed class LeetCodeAnswersTests
{
    [Fact]
    public void Exactly_OnArrays_ComparesByReferenceAndSoMustNotBeUsedForThem()
    {
        // Documents the footgun rather than the feature: this is exactly why
        // MatchingAnswersWith is required and never defaulted.
        Assert.False(LeetCodeAnswers.Exactly<int[]>([1, 2], [1, 2]));
    }

    [Fact]
    public void SequenceEqual_WhenOrderDiffers_Rejects()
    {
        Assert.True(LeetCodeAnswers.SequenceEqual<int>([1, 2, 3], [1, 2, 3]));
        Assert.False(LeetCodeAnswers.SequenceEqual<int>([1, 2, 3], [3, 2, 1]));
    }

    [Fact]
    public void SetEqual_WhenOrderDiffers_Accepts_ButRejectsADifferentMultiset()
    {
        Assert.True(LeetCodeAnswers.SetEqual<int>([1, 0], [0, 1]));
        Assert.False(LeetCodeAnswers.SetEqual<int>([1, 1], [0, 1]));
        Assert.False(LeetCodeAnswers.SetEqual<int>([1], [1, 1]));
    }

    [Fact]
    public void RowSetEqual_WhenRowsAreReordered_Accepts() =>
        Assert.True(LeetCodeAnswers.RowSetEqual<int>(
            [[3, 3], [-2, 4]],
            [[-2, 4], [3, 3]]));

    // The trap K Closest Points to Origin exists in the pilot to catch: the rows
    // are a set, but a row is a point, and reordering WITHIN a row is a different
    // point entirely.
    [Fact]
    public void RowSetEqual_WhenAPointsOwnCoordinatesAreSwapped_Rejects() =>
        Assert.False(LeetCodeAnswers.RowSetEqual<int>(
            [[3, -3]],
            [[-3, 3]]));

    [Fact]
    public void RowSetEqual_WhenRowCountsDiffer_Rejects() => Assert.False(LeetCodeAnswers.RowSetEqual<int>([[1, 1]], [[1, 1], [2, 2]]));

    [Fact]
    public void SequenceOfSequencesEqual_WhenRowsAreReordered_Rejects()
    {
        Assert.True(LeetCodeAnswers.SequenceOfSequencesEqual<int>([[1], [2, 3]], [[1], [2, 3]]));
        Assert.False(LeetCodeAnswers.SequenceOfSequencesEqual<int>([[2, 3], [1]], [[1], [2, 3]]));
    }

    [Fact]
    public void WithinTolerance_AtLeetCodesOwnStatedPrecision_AcceptsAndRejects()
    {
        Assert.True(LeetCodeAnswers.WithinTolerance(0.25, 0.2500001));
        Assert.False(LeetCodeAnswers.WithinTolerance(0.25, 0.26));
    }
}
