using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSortedVowelStringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - enumerating every sorted string against the memoized
// recurrence - so a harness whose arms disagree is timing two different problems. This benchmark has
// no [GlobalSetup]: StringLength is the whole workload, so there is nothing to rebuild.
public sealed partial class CountSortedVowelStringsBenchmarksTests
{
    private const int SmallestStringLength = 20;

    [Fact]
    public void BacktrackingEnumeration_SmallestStringLength_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.BacktrackingEnumeration());
    }

    [Fact]
    public void MemoizedRecurrence_SmallestStringLength_AgreesWithBacktrackingEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackingEnumeration(), harness.MemoizedRecurrence());
    }

    private static CountSortedVowelStringsBenchmarks BuildHarness() =>
        new() { StringLength = SmallestStringLength };
}
