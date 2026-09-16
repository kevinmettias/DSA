using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddStringsBenchmarks (ARCHITECTURE 17.9): its two arms differ only in how they undo the
// least-significant-first digit walk - a List<char> reversed at the end, or this repo's own Stack<char> - so a
// harness whose arms disagree is timing two different problems. Setup draws both operands from one fixed seed,
// so the same Length must rebuild the same two operands and with them the same sum; neither arm mutates its
// operands, so one harness instance is safe to read twice in either order.
public sealed partial class AddStringsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;
    private const char FirstDigit = '0';
    private const char LastDigit = '9';

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            AnswerText.Of(BuildHarness().CharArrayReverse()),
            AnswerText.Of(BuildHarness().CharArrayReverse()));
        Assert.Equal(
            AnswerText.Of(BuildHarness().StackDigits()),
            AnswerText.Of(BuildHarness().StackDigits()));
    }

    // Adding two Length-digit operands cannot outgrow a single carry digit, so the sum is either as long as
    // its operands or one digit longer - the bound the arms' digit buffers have to satisfy.
    [Fact]
    public void CharArrayReverse_RandomDigits_StaysWithinOneCarryDigitOfTheOperands()
    {
        var sum = BuildHarness().CharArrayReverse();

        Assert.InRange(sum.Length, FewestDigits, DigitsGrownByTheCarry);
        Assert.All(sum, digit => Assert.InRange(digit, FirstDigit, LastDigit));
    }

    [Fact]
    public void CharArrayReverse_RandomDigits_AgreesWithStackDigits()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StackDigits()), AnswerText.Of(harness.CharArrayReverse()));
    }

    [Fact]
    public void StackDigits_RandomDigits_AgreesWithCharArrayReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.CharArrayReverse()), AnswerText.Of(harness.StackDigits()));
    }

    private static AddStringsBenchmarks BuildHarness()
    {
        var harness = new AddStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
