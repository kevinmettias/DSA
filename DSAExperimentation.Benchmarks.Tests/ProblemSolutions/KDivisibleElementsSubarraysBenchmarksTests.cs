using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KDivisibleElementsSubarraysBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for one question - the number of distinct subarrays with at most
// MaxDivisibleCount elements divisible by DivisorP - so a harness whose arms disagree is timing
// two different problems. Both arms enumerate the same k-truncated candidate set and answer with
// the count of it, so agreement is on the deduplicated total rather than on a container's
// iteration order, which is what makes a bare int comparison meaningful here.
public sealed partial class KDivisibleElementsSubarraysBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueArray() =>
        Assert.Equal(
            BuildHarness().HashSetDeduped(),
            BuildHarness().HashSetDeduped());

    [Fact]
    public void HashSetDeduped_SeededValues_AgreesWithRepoSetDeduped()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoSetDeduped(), harness.HashSetDeduped());
    }

    [Fact]
    public void RepoSetDeduped_SeededValues_AgreesWithHashSetDeduped()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashSetDeduped(), harness.RepoSetDeduped());
    }

    private static KDivisibleElementsSubarraysBenchmarks BuildHarness()
    {
        var harness = new KDivisibleElementsSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
