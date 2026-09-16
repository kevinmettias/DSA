using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationSequenceBenchmarks (ARCHITECTURE 17.9): its two arms are
// PermutationSequenceSolution's, competing selections for the same permutation - enumerating the
// lexicographic order until the requested rank is reached against the factorial number system
// picking each digit directly - so a harness whose arms disagree is timing two different problems.
// Setup fixes the rank at DigitCount!, the lexicographically last permutation, which is what makes
// the expected string a fixture-derived constant rather than a value read off either arm; the
// tests pin it as well as the agreement.
public sealed partial class PermutationSequenceBenchmarksTests
{
    private const int SmallestDigitCount = 6;

    // The last permutation of the digits 1..DigitCount in lexicographic order, which is rank
    // DigitCount! - the descending arrangement.
    private const string LexicographicallyLastPermutation = "654321";

    [Fact]
    public void Setup_SameDigitCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BacktrackEnumeration(), BuildHarness().BacktrackEnumeration());

    [Fact]
    public void BacktrackEnumeration_LastLexicographicRank_AgreesWithFactoradicSelection()
    {
        var harness = BuildHarness();

        Assert.Equal(LexicographicallyLastPermutation, harness.FactoradicSelection());
        Assert.Equal(harness.FactoradicSelection(), harness.BacktrackEnumeration());
    }

    [Fact]
    public void FactoradicSelection_LastLexicographicRank_AgreesWithBacktrackEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(LexicographicallyLastPermutation, harness.BacktrackEnumeration());
        Assert.Equal(harness.BacktrackEnumeration(), harness.FactoradicSelection());
    }

    private static PermutationSequenceBenchmarks BuildHarness()
    {
        var harness = new PermutationSequenceBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
