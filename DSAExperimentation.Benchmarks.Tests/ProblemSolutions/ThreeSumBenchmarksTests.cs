using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThreeSumBenchmarks (ARCHITECTURE 17.9): LC 15 asks for the set of unique
// triplets and never fixes the sequence they are listed in, and the two arms really do use different
// ones - the brute force returns a HashSet's enumeration order, while the sweep emits triplets in
// ascending first-element order - so the generic arm check compares their answers, and each arm's
// answer across rebuilds, only as unordered sets.
//
// Setup draws the values from a fixed seed, so the same Length must rebuild the same workload; the
// same arm called on two harnesses must then reproduce the identical answer, order included - the
// one rebuild property the unordered comparison cannot see.
public sealed partial class ThreeSumBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().MergeSortTwoPointers()),
            AnswerGraphText.Of(BuildHarness().MergeSortTwoPointers()));

    private static ThreeSumBenchmarks BuildHarness()
    {
        var harness = new ThreeSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
