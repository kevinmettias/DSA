using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAddToMakeParenthesesValidBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a running-counter balance walk that stores
// nothing against this repo's Stack<char> holding each unmatched opener explicitly - so a harness
// whose arms disagree is timing two different problems. Setup draws the bracket string once from a
// seeded stream, so the same Length must rebuild the same string, and the random draw gives both
// arms a genuine mix of matched and unmatched positions rather than a trivially balanced string.
public sealed partial class MinimumAddToMakeParenthesesValidBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBracketString() =>
        Assert.Equal(BuildHarness().RunningCounter(), BuildHarness().RunningCounter());

    [Fact]
    public void RunningCounter_RandomBracketString_AgreesWithStackOfOpeners()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackOfOpeners(), harness.RunningCounter());
    }

    [Fact]
    public void StackOfOpeners_RandomBracketString_AgreesWithRunningCounter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningCounter(), harness.StackOfOpeners());
    }

    private static MinimumAddToMakeParenthesesValidBenchmarks BuildHarness()
    {
        var harness = new MinimumAddToMakeParenthesesValidBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
