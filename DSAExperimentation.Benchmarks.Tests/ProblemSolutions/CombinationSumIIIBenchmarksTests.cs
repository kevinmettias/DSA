using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CombinationSumIIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - enumerating every 5-subset of 1..9 against the pruning
// backtracking engine - so a harness whose arms disagree is timing two different problems. The class
// carries no [GlobalSetup]: CombinationSize is a fixed constant and TargetSum is the only [Params]
// property, and both arms read them directly, so the same TargetSum must produce the same
// combinations.
//
// AnswerText.OfUnorderedSet, not Of: LeetCode leaves the order of the returned combination set
// unspecified, while the order inside one combination is the ascending order the arms both build.
public sealed partial class CombinationSumIIIBenchmarksTests
{
    private const int SmallestTargetSum = 20;

    [Fact]
    public void BacktrackEngine_FiveOfOneThroughNineSummingToTwenty_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.BruteForce()),
            AnswerText.OfUnorderedSet(harness.BacktrackEngine()));
    }

    [Fact]
    public void BruteForce_FiveOfOneThroughNineSummingToTwenty_AgreesWithBacktrackEngine()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.BacktrackEngine()),
            AnswerText.OfUnorderedSet(harness.BruteForce()));
    }

    private static CombinationSumIIIBenchmarks BuildHarness() => new() { TargetSum = SmallestTargetSum };
}
