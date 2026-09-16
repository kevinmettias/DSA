using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfIntegersWithPopcountDepthEqualToKIBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - simulating every integer up to UpperBound
// against the popcount combinatorics - so a harness whose arms disagree is counting over two different
// ranges. The class carries no [GlobalSetup] and needs none: UpperBound and K are LeetCode's own input,
// passed straight through to both arms, so the same UpperBound must answer both.
//
// Both arms return a long, so they are compared directly. UpperBound stays at the smaller declared
// [Params] value because the brute-force arm would not finish at the real problem's 10^15 bound.
public sealed partial class NumberOfIntegersWithPopcountDepthEqualToKIBenchmarksTests
{
    private const long SmallestUpperBound = 10_000;

    [Fact]
    public void BruteForce_AgreesWithPopcountCombinatorics()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PopcountCombinatorics(), harness.BruteForce());
    }

    [Fact]
    public void PopcountCombinatorics_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.PopcountCombinatorics());
    }

    private static NumberOfIntegersWithPopcountDepthEqualToKIBenchmarks BuildHarness() =>
        new() { UpperBound = SmallestUpperBound };
}
