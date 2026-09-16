using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PlusOneBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for one question - the digits of the number plus one - so a harness whose arms disagree is timing
// two different problems. Setup hands both arms the all-nines operand the class comment names, so
// the carry cascades through every position; the same Length must rebuild that same operand.
public sealed partial class PlusOneBenchmarksTests
{
    private const int SmallestLength = 200;

    // The all-nines operand grows the number by exactly one digit.
    private const int GrownLength = SmallestLength + 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDigits() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().ArrayWalk()),
            AnswerText.Of(BuildHarness().ArrayWalk()));

    [Fact]
    public void ArrayWalk_AgreesWithDigitStack()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DigitStack()),
            AnswerText.Of(harness.ArrayWalk()));
    }

    [Fact]
    public void DigitStack_AgreesWithArrayWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ArrayWalk()),
            AnswerText.Of(harness.DigitStack()));
    }

    [Fact]
    public void ArrayWalk_AllNines_GrowsTheNumberByOneDigit() =>
        Assert.Equal(GrownLength, BuildHarness().ArrayWalk().Length);

    private static PlusOneBenchmarks BuildHarness()
    {
        var harness = new PlusOneBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
