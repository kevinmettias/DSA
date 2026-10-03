using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementIIBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems: the HashMap count's single pass and the two-candidate voting arm's pass plus recount
// must still name the same values. Setup seeds two disjoint values, each (Length / 3) + 1 times -
// a strict third apiece - and fills the rest with noise disjoint from both, so exactly those two
// values exceed the floor(n/3) frequency
// and nothing else does, and the same Length must rebuild the same array. AnswerGraphText.OfUnordered
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
            AnswerGraphText.OfUnordered(BuildHarness().HashMapCount()),
            AnswerGraphText.OfUnordered(BuildHarness().HashMapCount()));

    [Fact]
    public void HashMapCount_TwoSeededThirdsOverDisjointNoise_ReturnsBothSeededValues() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(ExpectedMajorityValues),
            AnswerGraphText.OfUnordered(BuildHarness().HashMapCount()));

    [Fact]
    public void ExtendedBoyerMooreVoting_TwoSeededThirdsOverDisjointNoise_ReturnsBothSeededValues() =>
        Assert.Equal(
            AnswerGraphText.OfUnordered(ExpectedMajorityValues),
            AnswerGraphText.OfUnordered(BuildHarness().ExtendedBoyerMooreVoting()));

    [Fact]
    public void ExtendedBoyerMooreVoting_AgreesWithHashMapCount()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.HashMapCount()),
            AnswerGraphText.OfUnordered(harness.ExtendedBoyerMooreVoting()));
    }

    private static MajorityElementIIBenchmarks BuildHarness()
    {
        var harness = new MajorityElementIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
