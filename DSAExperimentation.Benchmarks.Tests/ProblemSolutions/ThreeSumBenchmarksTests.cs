using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThreeSumBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - cubic duplicate-filtered brute force against MergeSort plus
// the sorted two-pointer sweep - so a harness whose arms disagree is timing two different problems.
//
// The genuinely unfixed order here is the OUTER one: LC 15 asks for the set of unique triplets and
// never fixes the sequence they are listed in. The two arms really do use different ones - the
// brute force returns a HashSet's enumeration order, while the sweep emits triplets in ascending
// first-element order - which the repo's own LeetCodeCoverage/ThreeSum tests document as outside
// either strategy's contract. Each triplet's own three values keep a fixed order (both arms sort
// into ascending first < second < third), so the set comparison still checks every triplet
// order-sensitively and cannot hide a disagreement about which triplets were found.
//
// Setup draws the values from a fixed seed, so the same Length must rebuild the same workload; the
// same arm called on two harnesses must then reproduce the identical answer, order included.
public sealed partial class ThreeSumBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().MergeSortTwoPointers()),
            AnswerText.Of(BuildHarness().MergeSortTwoPointers()));

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithMergeSortTwoPointers()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.MergeSortTwoPointers()),
            AnswerText.OfUnorderedSet(harness.BruteForce()));
    }

    [Fact]
    public void MergeSortTwoPointers_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.BruteForce()),
            AnswerText.OfUnorderedSet(harness.MergeSortTwoPointers()));
    }

    private static ThreeSumBenchmarks BuildHarness()
    {
        var harness = new ThreeSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
