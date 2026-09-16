using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubstringMatchingPatternBenchmarks (ARCHITECTURE 17.9): both arms are
// SubstringMatchingPatternSolution's, so a harness whose arms disagree is timing two different
// questions. Both answer with a bare bool, and the workload [GlobalSetup] builds makes that bool
// decisive rather than merely agreed: subject is a run of 'a's while both halves of pattern are a run
// of 'a's followed by a 'b', a character subject never contains, so no window of subject can start
// with the prefix and the honest verdict is false - which each arm is pinned to next to the
// agreement, since two arms that were wrong in the same way would still agree.
public sealed partial class SubstringMatchingPatternBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().HasMatchByBruteForce(), BuildHarness().HasMatchByBruteForce());

    [Fact]
    public void HasMatchByBruteForce_HalvesEndingInAnAbsentCharacter_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasMatchByPrefixFunctionSearch(), harness.HasMatchByBruteForce());
        Assert.False(harness.HasMatchByBruteForce());
    }

    [Fact]
    public void HasMatchByPrefixFunctionSearch_HalvesEndingInAnAbsentCharacter_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasMatchByBruteForce(), harness.HasMatchByPrefixFunctionSearch());
        Assert.False(harness.HasMatchByPrefixFunctionSearch());
    }

    private static SubstringMatchingPatternBenchmarks BuildHarness()
    {
        var harness = new SubstringMatchingPatternBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
