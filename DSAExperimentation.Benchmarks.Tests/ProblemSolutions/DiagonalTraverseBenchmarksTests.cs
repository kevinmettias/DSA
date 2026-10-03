using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DiagonalTraverseBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - one cursor bouncing between the two directions against
// grouping cells by diagonal and reversing every other group - so a harness whose arms disagree is
// timing two different problems. AnswerGraphText.Of, not OfUnordered: the returned order IS the
// answer here, because reading the matrix in the zig-zag diagonal order is the whole question.
// The matrix is Size x Size by construction, so the answer always holds exactly Size * Size entries.
public sealed partial class DiagonalTraverseBenchmarksTests
{
    private const int SmallestSize = 50;
    private const int ExpectedDiagonalOrderLength = SmallestSize * SmallestSize;

    [Fact]
    public void Setup_SquareMatrix_HoldsOneEntryPerCellAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var order = harness.DirectionToggleWalk();

        Assert.Equal(ExpectedDiagonalOrderLength, order.Length);
        Assert.Equal(AnswerGraphText.Of(order), AnswerGraphText.Of(BuildHarness().DirectionToggleWalk()));
    }

    [Fact]
    public void DirectionToggleWalk_ZigZagOverASquareMatrix_AgreesWithDiagonalGroupsWithStackReversal()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.DiagonalGroupsWithStackReversal()),
            AnswerGraphText.Of(harness.DirectionToggleWalk()));
    }

    [Fact]
    public void DiagonalGroupsWithStackReversal_ZigZagOverASquareMatrix_AgreesWithDirectionToggleWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.DirectionToggleWalk()),
            AnswerGraphText.Of(harness.DiagonalGroupsWithStackReversal()));
    }

    private static DiagonalTraverseBenchmarks BuildHarness()
    {
        var harness = new DiagonalTraverseBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
