using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlippingAnImageBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup draws the image from a fixed seed, so the same Side must rebuild the same workload,
// and each arm clones that image before rewriting it in place - so one harness instance is safe to
// call twice in either order.
//
// Both arms rewrite the same rows in place, so the returned matrix is positional and
// AnswerText.Of's order-sensitive rendering is the right comparison.
public sealed partial class FlippingAnImageBenchmarksTests
{
    private const int SmallestSide = 50;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().TwoPointerReverseAndInvert()),
            AnswerText.Of(BuildHarness().TwoPointerReverseAndInvert()));

    [Fact]
    public void TwoPointerReverseAndInvert_AgreesWithStackReverseAndInvert()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StackReverseAndInvert()),
            AnswerText.Of(harness.TwoPointerReverseAndInvert()));
    }

    [Fact]
    public void StackReverseAndInvert_AgreesWithTwoPointerReverseAndInvert()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.TwoPointerReverseAndInvert()),
            AnswerText.Of(harness.StackReverseAndInvert()));
    }

    private static FlippingAnImageBenchmarks BuildHarness()
    {
        var harness = new FlippingAnImageBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
