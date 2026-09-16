using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementIIBenchmarks (ARCHITECTURE 17.9). There is one arm, so there
// is no second strategy to agree with: the expected answer comes from the fixture instead. Setup
// seeds two disjoint values, each (Length / 3) + 1 times - a strict third apiece - and fills the
// rest with noise disjoint from both, so exactly those two values exceed the floor(n/3) frequency
// and nothing else does, and the same Length must rebuild the same array. AnswerText.OfUnorderedSet
// renders that pair as a set because LC 229 fixes no order on the values it returns (its own
// examples list them however the scan happens to find them), so the outer order here is genuinely
// unfixed rather than a disagreement being papered over; each value's own rendering is unchanged.
public sealed partial class MajorityElementIIBenchmarksTests
{
    private const int SmallestLength = 200;

    private static readonly int[] ExpectedMajorityValues = [-2, -1];

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().HashMapCount()),
            AnswerText.OfUnorderedSet(BuildHarness().HashMapCount()));

    [Fact]
    public void HashMapCount_TwoSeededThirdsOverDisjointNoise_ReturnsBothSeededValues() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(ExpectedMajorityValues),
            AnswerText.OfUnorderedSet(BuildHarness().HashMapCount()));

    private static MajorityElementIIBenchmarks BuildHarness()
    {
        var harness = new MajorityElementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
