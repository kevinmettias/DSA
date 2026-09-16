using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks (ARCHITECTURE
// 17.9): both arms are MaximumNestingDepthOfTwoValidParenthesesStringsSolution's competing
// strategies for one question - the O(n^2) per-position depth rescan against the single-pass
// Stack<char> walk - so a harness whose arms disagree is timing two different problems.
//
// Each arm answers with one assignment per character of the sequence, which is part of this
// answer: the value at index i belongs to character i, so AnswerText.Of and not OfUnorderedSet is
// the rendering that keeps each assignment scored against its own position.
public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValidSequence()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // One assignment per character is the shape both arms promise, so the rebuilt workload is
        // pinned through the assignment they produce: the same Length must generate the same
        // seeded sequence and split it identically.
        Assert.Equal(SmallestLength, first.RecomputeDepthPerPosition().Length);
        Assert.Equal(
            AnswerText.Of(first.StackTrackedSinglePass()),
            AnswerText.Of(second.StackTrackedSinglePass()));
    }

    [Fact]
    public void RecomputeDepthPerPosition_SeededValidSequence_AgreesWithStackTrackedSinglePass()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StackTrackedSinglePass()),
            AnswerText.Of(harness.RecomputeDepthPerPosition()));
    }

    [Fact]
    public void StackTrackedSinglePass_SeededValidSequence_AgreesWithRecomputeDepthPerPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RecomputeDepthPerPosition()),
            AnswerText.Of(harness.StackTrackedSinglePass()));
    }

    private static MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks BuildHarness()
    {
        var harness = new MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
