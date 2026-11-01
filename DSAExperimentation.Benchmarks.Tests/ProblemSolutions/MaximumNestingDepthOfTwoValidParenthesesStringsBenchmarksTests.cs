using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks (ARCHITECTURE
// 17.9): each arm answers with one assignment per character of the sequence - the value at index i
// belongs to character i - and Setup generates a Length-character sequence. That shape is a property
// of the workload the generic agreement check, which only compares arms with each other, cannot
// supply, so the baseline's assignment count is pinned to it.
public sealed partial class MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarksTests
{
    private const int SmallestLength = 200;

    // One assignment per character is the shape both arms promise.
    [Fact]
    public void RecomputeDepthPerPosition_SeededValidSequence_AssignsEveryCharacter() =>
        Assert.Equal(SmallestLength, BuildHarness().RecomputeDepthPerPosition().Length);

    private static MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks BuildHarness()
    {
        var harness = new MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
