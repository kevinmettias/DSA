using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CrackingTheSafeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a hand-specialized greedy-with-undo walk over the de Bruijn graph
// against the identical walk composed from this repo's generic backtracking engine - so a harness whose
// arms disagree is timing two different problems. The class has no [GlobalSetup]: K is a constant and
// the whole workload is the single [Params] value, so the smallest PasswordLength is 2 - a real search
// whose answer must cover all four two-digit passwords - rather than a degenerate one.
public sealed partial class CrackingTheSafeBenchmarksTests
{
    private const int SmallestPasswordLength = 2;

    [Fact]
    public void GreedyRecursion_BinaryAlphabetLengthTwo_AgreesWithBacktrackEngine()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackEngine(), harness.GreedyRecursion());
    }

    [Fact]
    public void BacktrackEngine_BinaryAlphabetLengthTwo_AgreesWithGreedyRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedyRecursion(), harness.BacktrackEngine());
    }

    private static CrackingTheSafeBenchmarks BuildHarness() =>
        new() { PasswordLength = SmallestPasswordLength };
}
