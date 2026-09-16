using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for VowelsOfAllSubstringsBenchmarks (ARCHITECTURE 17.9): its two arms are
// VowelsOfAllSubstringsSolution's competing strategies for the same question - the O(n^2) scan that
// counts each substring's vowels against the per-character contribution formula - so a harness whose
// arms disagree is summing two different strings.
//
// Both arms answer with LC 2063's own quantity, the total vowel count over every substring, which is
// the whole answer rather than a proxy for it, so agreeing on it is agreeing on everything. Setup
// builds the word from the shared VowelsOfAllSubstringsWorkloads fixture off one fixed seed, which
// is why the same Length must rebuild the same word: the total is derived from the letters, so
// reproducing it is what pins the rebuild.
public sealed partial class VowelsOfAllSubstringsBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ContributionFormula(), BuildHarness().ContributionFormula());

    [Fact]
    public void SubstringScan_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ContributionFormula(), harness.SubstringScan());
    }

    [Fact]
    public void ContributionFormula_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SubstringScan(), harness.ContributionFormula());
    }

    private static VowelsOfAllSubstringsBenchmarks BuildHarness()
    {
        var harness = new VowelsOfAllSubstringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
