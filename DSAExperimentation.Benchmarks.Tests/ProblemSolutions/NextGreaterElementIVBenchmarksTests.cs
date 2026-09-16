using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NextGreaterElementIVBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) brute-force scan that walks forward until it has seen
// two greater values against the O(n) sweep that resolves indices from two monotonic Stack<int>s - so
// a harness whose arms disagree is answering two different second-greater-element queries. Setup
// draws the seeded value array, so the same Length must rebuild the same values.
//
// Each index must be resolved by a STRICTLY greater later value, which is what makes the two-stack
// bookkeeping more than a single sweep; the values repeat, so ties are genuinely present in the
// workload. Neither arm mutates the array, so one harness serves both in either order.
public sealed partial class NextGreaterElementIVBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithTwoMonotonicStacks()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TwoMonotonicStacks()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void TwoMonotonicStacks_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.TwoMonotonicStacks()));
    }

    private static NextGreaterElementIVBenchmarks BuildHarness()
    {
        var harness = new NextGreaterElementIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
