using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - an O(n*k)
// rescan that restarts the window scan for every output character against a single O(n)
// monotonic-stack sweep - so a harness whose arms disagree is scanning two different texts.
// Setup builds the text and chooses the subsequence length from one fixed seed, so the same
// Length must rebuild the same workload; otherwise two published numbers were never comparable.
// Setup also plants its required repetitions of the letter at both ends of the text, so a
// feasible subsequence exists at either tuning and neither arm may answer with the problem's
// "impossible" sentinel.
public sealed partial class SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameText() =>
        Assert.Equal(BuildHarness().NaiveWindowRescan(), BuildHarness().NaiveWindowRescan());

    [Fact]
    public void NaiveWindowRescan_FiveHundredCharacterText_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackSweep(), harness.NaiveWindowRescan());
    }

    [Fact]
    public void MonotonicStackSweep_FiveHundredCharacterText_AgreesWithNaiveWindowRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveWindowRescan(), harness.MonotonicStackSweep());
    }

    private static SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarks BuildHarness()
    {
        var harness = new SmallestKLengthSubsequenceWithOccurrencesOfALetterBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
