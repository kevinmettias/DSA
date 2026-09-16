using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MakeLexicographicallySmallestArrayBySwappingElementsBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - sorting once
// and swapping within contiguous value groups against labelling those groups with a disjoint set -
// so a harness whose arms disagree is timing two different problems. The lexicographically smallest
// arrangement of a fixed value multiset is unique, so the two arrays are compared positionally with
// AnswerText.Of. Setup draws the values from one fixed seed, so the same Length must rebuild the
// same array and the same answer.
public sealed partial class MakeLexicographicallySmallestArrayBySwappingElementsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerText.Of(BuildHarness().ContiguousGroups()),
            AnswerText.Of(BuildHarness().ContiguousGroups()));
        Assert.Equal(
            AnswerText.Of(BuildHarness().DisjointSet()),
            AnswerText.Of(BuildHarness().DisjointSet()));
    }

    [Fact]
    public void ContiguousGroups_SmallLimitOverWideValueRange_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DisjointSet()),
            AnswerText.Of(harness.ContiguousGroups()));
    }

    [Fact]
    public void DisjointSet_SmallLimitOverWideValueRange_AgreesWithContiguousGroups()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ContiguousGroups()),
            AnswerText.Of(harness.DisjointSet()));
    }

    private static MakeLexicographicallySmallestArrayBySwappingElementsBenchmarks BuildHarness()
    {
        var harness = new MakeLexicographicallySmallestArrayBySwappingElementsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
