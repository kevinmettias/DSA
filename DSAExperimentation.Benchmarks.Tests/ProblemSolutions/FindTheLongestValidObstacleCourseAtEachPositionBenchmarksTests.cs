using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheLongestValidObstacleCourseAtEachPositionBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for one question - the quadratic DP against
// patience sorting over the repo's UpperBound - so a harness whose arms disagree is timing two
// different problems. Both answers are one length per index, so AnswerText.Of (not
// OfUnorderedSet) is what keeps a result from being scored against the wrong obstacle. Setup draws
// the height sequence from one fixed seed, so the same Length must rebuild the same array.
public sealed partial class FindTheLongestValidObstacleCourseAtEachPositionBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DynamicProgramming()),
            AnswerText.Of(BuildHarness().DynamicProgramming()));

    [Fact]
    public void DynamicProgramming_SmallestLength_AgreesWithPatienceSortingBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.PatienceSortingBinarySearch()),
            AnswerText.Of(harness.DynamicProgramming()));
    }

    [Fact]
    public void PatienceSortingBinarySearch_SmallestLength_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DynamicProgramming()),
            AnswerText.Of(harness.PatienceSortingBinarySearch()));
    }

    private static FindTheLongestValidObstacleCourseAtEachPositionBenchmarks BuildHarness()
    {
        var harness = new FindTheLongestValidObstacleCourseAtEachPositionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
