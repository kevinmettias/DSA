using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MultiplyStringsBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - parsing both operands as machine integers against the Stack<char>-based
// digit-by-digit multiply - so a harness whose arms disagree is timing two different problems, and on this
// workload they are asked the same question because the [Params] digit counts stay short enough that the
// integer shortcut still answers it. Both arms only read the two generated operands, so one harness instance
// is safe to call twice in either order. Setup draws both from one fixed seed, so the same Digits must rebuild
// the same operands; otherwise two published numbers were never comparable in the first place.
public sealed partial class MultiplyStringsBenchmarksTests
{
    private const int SmallestDigits = 5;

    [Fact]
    public void Setup_SameDigits_RebuildsTheSameOperands() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LongConversion()),
            AnswerText.Of(BuildHarness().LongConversion()));

    [Fact]
    public void LongConversion_FiveDigitOperands_AgreesWithStackDigitByDigit()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.StackDigitByDigit()),
            AnswerText.Of(harness.LongConversion()));
    }

    [Fact]
    public void StackDigitByDigit_FiveDigitOperands_AgreesWithLongConversion()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LongConversion()),
            AnswerText.Of(harness.StackDigitByDigit()));
    }

    private static MultiplyStringsBenchmarks BuildHarness()
    {
        var harness = new MultiplyStringsBenchmarks { Digits = SmallestDigits };
        harness.Setup();

        return harness;
    }
}
