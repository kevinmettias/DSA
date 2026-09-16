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
        var byReference = LeetCodeAnswers.Exactly<int[]>([1, 2], [1, 2]);

        Assert.False(byReference);
    }

    [Fact]
    public void SequenceEqual_WhenOrderDiffers_Rejects()
    {
        var sameOrder = LeetCodeAnswers.SequenceEqual<int>([1, 2, 3], [1, 2, 3]);
        var reversedOrder = LeetCodeAnswers.SequenceEqual<int>([1, 2, 3], [3, 2, 1]);

        Assert.True(sameOrder);
        Assert.False(reversedOrder);
    }

    [Fact]
    public void SetEqual_WhenOrderDiffers_Accepts_ButRejectsADifferentMultiset()
    {
        var reordered = LeetCodeAnswers.SetEqual<int>([1, 0], [0, 1]);
        var differentMultiset = LeetCodeAnswers.SetEqual<int>([1, 1], [0, 1]);
        var differentSize = LeetCodeAnswers.SetEqual<int>([1], [1, 1]);

        Assert.True(reordered);
        Assert.False(differentMultiset);
        Assert.False(differentSize);
    }

    [Fact]
    public void RowSetEqual_WhenRowsAreReordered_Accepts()
    {
        var reordered = LeetCodeAnswers.RowSetEqual<int>(
            [[3, 3], [-2, 4]],
            [[-2, 4], [3, 3]]);

        Assert.True(reordered);
    }

    // The trap K Closest Points to Origin exists in the pilot to catch: the rows
    // are a set, but a row is a point, and reordering WITHIN a row is a different
    // point entirely.
    [Fact]
    public void RowSetEqual_WhenAPointsOwnCoordinatesAreSwapped_Rejects()
    {
        var coordinatesSwapped = LeetCodeAnswers.RowSetEqual<int>(
            [[3, -3]],
            [[-3, 3]]);

        Assert.False(coordinatesSwapped);
    }

    [Fact]
    public void RowSetEqual_WhenRowCountsDiffer_Rejects()
    {
        var differentRowCounts = LeetCodeAnswers.RowSetEqual<int>([[1, 1]], [[1, 1], [2, 2]]);

        Assert.False(differentRowCounts);
    }

    [Fact]
    public void SequenceOfSequencesEqual_WhenRowsAreReordered_Rejects()
    {
        var sameSequence = LeetCodeAnswers.SequenceOfSequencesEqual<int>([[1], [2, 3]], [[1], [2, 3]]);
        var reorderedRows = LeetCodeAnswers.SequenceOfSequencesEqual<int>([[2, 3], [1]], [[1], [2, 3]]);

        Assert.True(sameSequence);
        Assert.False(reorderedRows);
    }

    [Fact]
    public void WithinTolerance_AtLeetCodesOwnStatedPrecision_AcceptsAndRejects()
    {
        var withinTolerance = LeetCodeAnswers.WithinTolerance(0.25, 0.2500001);
        var outsideTolerance = LeetCodeAnswers.WithinTolerance(0.25, 0.26);

        Assert.True(withinTolerance);
        Assert.False(outsideTolerance);
    }
}
