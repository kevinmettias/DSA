using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BasicCalculatorWorkloads (ARCHITECTURE 17.7). What the reading depends on
// is that the requested length is actually reached and that the generated groups nest only one
// level deep, which is what keeps the recursive-descent baseline's recursion depth constant as
// Length grows instead of overflowing the stack.
public sealed partial class BasicCalculatorWorkloadsTests
{
    private const int Length = 4_096;
    private const int MaxNestingDepth = 1;
    private const string ExpressionAlphabet = "0123456789()+-";

    [Fact]
    public void BuildExpression_RequestedLength_ReturnsAtLeastThatManyCharacters() =>
        Assert.True(BasicCalculatorWorkloads.BuildExpression(Length).Length >= Length);

    [Fact]
    public void BuildExpression_EveryPrefix_KeepsNestingAtMostOneLevelDeep()
    {
        var depth = 0;

        foreach (var character in BasicCalculatorWorkloads.BuildExpression(Length))
        {
            depth += NestingStep(character);
            Assert.InRange(depth, 0, MaxNestingDepth);
        }
    }

    [Fact]
    public void BuildExpression_WholeExpression_ClosesEveryGroupItOpens()
    {
        var expression = BasicCalculatorWorkloads.BuildExpression(Length);

        Assert.Equal(
            expression.Count(character => character == '('),
            expression.Count(character => character == ')'));
    }

    [Fact]
    public void BuildExpression_EveryCharacter_IsAnOperandDigitAnOperatorOrAGroupDelimiter() =>
        Assert.All(
            BasicCalculatorWorkloads.BuildExpression(Length),
            character => Assert.True(ExpressionAlphabet.Contains(character)));

    [Fact]
    public void BuildExpression_SameLength_ReturnsTheSameExpression() =>
        Assert.Equal(
            BasicCalculatorWorkloads.BuildExpression(Length),
            BasicCalculatorWorkloads.BuildExpression(Length));

    private static int NestingStep(char character) =>
        character switch
        {
            '(' => 1,
            ')' => -1,
            _ => 0,
        };
}
