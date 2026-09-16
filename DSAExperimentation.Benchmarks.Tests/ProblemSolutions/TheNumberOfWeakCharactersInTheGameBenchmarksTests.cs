using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheNumberOfWeakCharactersInTheGameBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the literal O(n^2) pairwise check against
// sorting via this repo's MergeSort and making one linear pass - so a harness whose arms disagree
// is timing two different problems. Both arms return the weak-character count as an int, so they
// are compared directly. Setup builds the CharacterRoster both arms take from a fixed seed, so the
// same Length must rebuild the same roster; the narrow attack range keeps many characters tied on
// attack, which is what exercises the descending-attack/ascending-defense tie-break.
public sealed partial class TheNumberOfWeakCharactersInTheGameBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameRoster() =>
        Assert.Equal(BuildHarness().PairwiseComparison(), BuildHarness().PairwiseComparison());

    [Fact]
    public void PairwiseComparison_SmallestLength_AgreesWithSortThenScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortThenScan(), harness.PairwiseComparison());
    }

    [Fact]
    public void SortThenScan_SmallestLength_AgreesWithPairwiseComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseComparison(), harness.SortThenScan());
    }

    private static TheNumberOfWeakCharactersInTheGameBenchmarks BuildHarness()
    {
        var harness = new TheNumberOfWeakCharactersInTheGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
