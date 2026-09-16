using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfGroupsGettingFreshDonutsBenchmarks (ARCHITECTURE 17.9): both
// arms are MaximumNumberOfGroupsGettingFreshDonutsSolution's competing strategies for one question
// - scoring every one of the n! orderings against the memoized (residue, remaining remainder
// counts) search - so a harness whose arms disagree is timing two different problems. Both answer
// with a single happy-group count, compared directly.
public sealed partial class MaximumNumberOfGroupsGettingFreshDonutsBenchmarksTests
{
    private const int SmallestGroupCount = 6;

    [Fact]
    public void Setup_SameGroupCount_RebuildsTheSameGroupSizes()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The group sizes are private, so the rebuild is pinned through the count they produce:
        // the same GroupCount must draw the same seeded sizes and schedule them identically.
        Assert.Equal(first.AllPermutations(), second.AllPermutations());
        Assert.Equal(first.MemoizedSearch(), second.MemoizedSearch());
    }

    [Fact]
    public void AllPermutations_SeededGroupSizes_AgreesWithMemoizedSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedSearch(), harness.AllPermutations());
    }

    [Fact]
    public void MemoizedSearch_SeededGroupSizes_AgreesWithAllPermutations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AllPermutations(), harness.MemoizedSearch());
    }

    private static MaximumNumberOfGroupsGettingFreshDonutsBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfGroupsGettingFreshDonutsBenchmarks { GroupCount = SmallestGroupCount };
        harness.Setup();

        return harness;
    }
}
