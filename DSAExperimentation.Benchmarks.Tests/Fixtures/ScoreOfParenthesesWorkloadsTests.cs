using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ScoreOfParenthesesWorkloads (ARCHITECTURE 17.7), shared by LC 856, LC 1021
// and LC 1614. The reading depends on the string genuinely being a balanced-parentheses expression
// of exactly pairCount atomic pairs - all three solutions score or strip a valid expression, and an
// invalid one would make their disagreements a matter of input outside the problem rather than of
// strategy - and on the nesting depth staying under the cap, which is what keeps the baseline's
// 1 << depth arithmetic from overflowing int.
public sealed partial class ScoreOfParenthesesWorkloadsTests
{
    private const int PairCount = 100;
    private const int MaxDepth = 10;
    private const int Seed = 856; // LC problem number
    private const int CharsPerPair = 2;
    private const char OpenParen = '(';
    private const char CloseParen = ')';
    private const int ClosedDepth = 0;
    private const string NeverClosesBeforeItOpens = "A balanced expression never closes before it opens.";

    [Fact]
    public void BuildBalanced_PairCount_ReturnsTwoCharactersPerPair() =>
        Assert.Equal(
            PairCount * CharsPerPair,
            ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed).Length);

    [Fact]
    public void BuildBalanced_PairCount_ReturnsExactlyOneOpenPerPair()
    {
        var expression = ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed);

        Assert.Equal(PairCount, expression.Count(character => character == OpenParen));
        Assert.Equal(PairCount, expression.Count(character => character == CloseParen));
    }

    // A prefix that closed more than it opened would be an expression no arm is specified for.
    [Fact]
    public void BuildBalanced_EveryPrefix_NeverClosesMoreThanItOpened()
    {
        var expression = ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed);
        var depth = 0;

        foreach (var character in expression)
        {
            depth += character == OpenParen ? 1 : -1;
            Assert.True(depth >= ClosedDepth, NeverClosesBeforeItOpens);
        }

        Assert.Equal(ClosedDepth, depth);
    }

    [Fact]
    public void BuildBalanced_NestingDepth_StaysInsideTheRequestedCap() =>
        Assert.InRange(
            MaximumDepth(ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed)),
            ClosedDepth,
            MaxDepth);

    [Fact]
    public void BuildBalanced_SameSeed_ReturnsTheSameExpression() =>
        Assert.Equal(
            ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed),
            ScoreOfParenthesesWorkloads.BuildBalanced(PairCount, MaxDepth, Seed));

    private static int MaximumDepth(string expression)
    {
        var depth = 0;
        var maximum = 0;

        foreach (var character in expression)
        {
            depth += character == OpenParen ? 1 : -1;
            maximum = Math.Max(maximum, depth);
        }

        return maximum;
    }
}
