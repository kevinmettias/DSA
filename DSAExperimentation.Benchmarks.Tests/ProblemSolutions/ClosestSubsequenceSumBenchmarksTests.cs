using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClosestSubsequenceSumBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - enumerating all 2^n subsets against meet-in-the-middle
// over two halves - so a harness whose arms disagree is timing two different problems. Setup draws
// the array from one fixed seed, so the same Length must rebuild the same array; otherwise two
// published numbers were never comparable in the first place. Both arms return a plain int, so the
// agreement assertion is the two calls compared directly.
public sealed partial class ClosestSubsequenceSumBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceAllSubsets(), BuildHarness().BruteForceAllSubsets());

    [Fact]
    public void BruteForceAllSubsets_GoalBeyondTheReachableSumRange_AgreesWithMeetInTheMiddle()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MeetInTheMiddle(), harness.BruteForceAllSubsets());
    }

    [Fact]
    public void MeetInTheMiddle_GoalBeyondTheReachableSumRange_AgreesWithBruteForceAllSubsets()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceAllSubsets(), harness.MeetInTheMiddle());
    }

    private static ClosestSubsequenceSumBenchmarks BuildHarness()
    {
        var harness = new ClosestSubsequenceSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
