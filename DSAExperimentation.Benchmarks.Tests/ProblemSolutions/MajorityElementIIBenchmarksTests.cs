using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementIIBenchmarks (ARCHITECTURE 17.9): Setup seeds two disjoint
// values, each (Length / 3) + 1 times - a strict third apiece - and fills the rest with noise
// disjoint from both, so exactly those two values exceed the floor(n/3) frequency and nothing else
// does - an expected answer derived from the workload's construction rather than from either arm,
// which the generic agreement check cannot supply. AnswerGraphText.OfUnordered renders that pair as
// a set because LC 229 fixes no order on the values it returns (its own examples list them however
// the scan happens to find them); each value's own rendering is unchanged.
public sealed partial class MajorityElementIIBenchmarksTests
{
    private const int SmallestLength = 200;

    private static readonly int[] ExpectedMajorityValues = [-2, -1];

    [Fact]
    public void ExtendedBoyerMooreVoting_TwoSeededThirdsOverDisjointNoise_ReturnsBothSeededValues() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(ExpectedMajorityValues),
            AnswerGraphText.OfUnordered(BuildHarness().ExtendedBoyerMooreVoting()));

    private static MajorityElementIIBenchmarks BuildHarness()
    {
        var harness = new MajorityElementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
