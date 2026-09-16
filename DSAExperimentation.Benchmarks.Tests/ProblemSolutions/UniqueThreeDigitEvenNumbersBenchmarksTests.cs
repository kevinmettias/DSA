using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniqueThreeDigitEvenNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniqueThreeDigitEvenNumbersSolution's competing strategies for the same question - the index
// permutation scan against the digit backtracker - so a harness whose arms disagree is counting two
// different sets of numbers.
//
// Both arms answer with the number of distinct three-digit even numbers the seeded digit multiset
// can form, which is LC 3483's own quantity rather than a proxy for it, so agreement is agreement on
// the whole answer. Setup draws the digits from one fixed seed, which is why the same DigitCount
// must rebuild the same count and a second, independently built harness reproduces it.
public sealed partial class UniqueThreeDigitEvenNumbersBenchmarksTests
{
    // The smaller of Setup's [Params(5, 10)] digit counts.
    private const int SmallestDigitCount = 5;

    [Fact]
    public void Setup_SameDigitCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().IndexPermutationScan(), BuildHarness().IndexPermutationScan());

    [Fact]
    public void IndexPermutationScan_SmallestDigitCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Backtracking(), harness.IndexPermutationScan());
    }

    [Fact]
    public void Backtracking_SmallestDigitCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IndexPermutationScan(), harness.Backtracking());
    }

    private static UniqueThreeDigitEvenNumbersBenchmarks BuildHarness()
    {
        var harness = new UniqueThreeDigitEvenNumbersBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
