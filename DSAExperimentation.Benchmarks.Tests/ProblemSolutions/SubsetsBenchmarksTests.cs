using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubsetsBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question, so a harness whose arms disagree is timing two different problems. Subset
// order is not part of the problem's contract, so the answers are compared as unordered sets of
// subsets - the same rendering SubsetsSolutionTests uses. Setup's workload is seeded, so the same element
// count must rebuild the same values.
public sealed partial class SubsetsBenchmarksTests
{
    private const int SmallestElementCount = 8;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(BuildHarness().BitmaskEnumeration()),
            AnswerGraphText.OfUnordered(BuildHarness().BitmaskEnumeration()));

    [Fact]
    public void BacktrackSearch_AgreesWithBitmaskEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.BitmaskEnumeration()),
            AnswerGraphText.OfUnordered(harness.BacktrackSearch()));
    }

    private static SubsetsBenchmarks BuildHarness()
    {
        var harness = new SubsetsBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
