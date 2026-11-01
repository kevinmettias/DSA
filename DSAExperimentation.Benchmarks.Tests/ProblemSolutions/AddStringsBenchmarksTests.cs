using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddStringsBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each other
// on the seeded operands, and this bounds the baseline's answer by what addition itself allows - a property
// no comparison between arms can establish.
public sealed partial class AddStringsBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;
    private const char FirstDigit = '0';
    private const char LastDigit = '9';

    // Adding two Length-digit operands cannot outgrow a single carry digit, so the sum is either as long as
    // its operands or one digit longer - the bound the arms' digit buffers have to satisfy.
    [Fact]
    public void CharArrayReverse_RandomDigits_StaysWithinOneCarryDigitOfTheOperands()
    {
        var sum = BuildHarness().CharArrayReverse();

        Assert.InRange(sum.Length, FewestDigits, DigitsGrownByTheCarry);
        Assert.All(sum, digit => Assert.InRange(digit, FirstDigit, LastDigit));
    }

    private static AddStringsBenchmarks BuildHarness()
    {
        var harness = new AddStringsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
