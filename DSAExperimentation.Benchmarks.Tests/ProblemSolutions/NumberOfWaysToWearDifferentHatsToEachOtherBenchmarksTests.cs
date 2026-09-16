using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for one count - the brute-force recursion against the memoized
// bitmask one - so a harness whose arms disagree is timing two different problems. Setup builds the
// HatPreferences from a seeded generator, so the same PeopleCount must produce the same preferences;
// the smallest tuned PeopleCount keeps the brute-force arm cheap to call twice.
public sealed partial class NumberOfWaysToWearDifferentHatsToEachOtherBenchmarksTests
{
    private const int SmallestPeopleCount = 5;

    [Fact]
    public void Setup_SamePeopleCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_SmallestPeopleCount_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SmallestPeopleCount_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks BuildHarness()
    {
        var harness = new NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks { PeopleCount = SmallestPeopleCount };
        harness.Setup();

        return harness;
    }
}
