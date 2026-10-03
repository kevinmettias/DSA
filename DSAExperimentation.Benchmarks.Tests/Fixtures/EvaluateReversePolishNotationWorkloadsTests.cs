using System.Globalization;
using DSAExperimentation.Benchmarks.Fixtures;
using static DSAExperimentation.Benchmarks.Fixtures.EvaluateReversePolishNotationWorkloads;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for EvaluateReversePolishNotationWorkloads (ARCHITECTURE 17.7). The fixture's
// comment promises LC 150's own guarantees - a valid postfix expression, operands inside [-200, 200],
// no division by zero, every intermediate value inside a 32-bit int - and returns the value the
// expression was built to have, which LC 150's benchmark pins its arm against. Replaying the tokens
// on a stack of 64-bit values checks every one of those promises at once, at LeetCode's
// 10,000-token cap, rather than trusting the value the generator tracked while building.
public sealed partial class EvaluateReversePolishNotationWorkloadsTests
{
    private const int OperandCount = 5_000;
    private const int Seed = 150; // LC problem number
    private const int MaxOperand = 200;
    private const int OperandsPerOperator = 2;

    [Fact]
    public void Build_OperandCount_ReturnsThatManyOperandsAndOneFewerOperators()
    {
        var tokens = Build().Tokens;

        Assert.Equal((OperandsPerOperator * OperandCount) - 1, tokens.Length);
        Assert.Equal(OperandCount, tokens.Count(IsOperand));
    }

    [Fact]
    public void Build_EveryOperand_StaysWithinLeetCodesRange() =>
        Assert.All(
            Build().Tokens.Where(IsOperand),
            token => Assert.InRange(int.Parse(token, CultureInfo.InvariantCulture), -MaxOperand, MaxOperand));

    [Fact]
    public void Build_Value_IsWhatTheTokensEvaluateTo()
    {
        var (tokens, value) = Build();

        Assert.Equal(value, Replay(tokens));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameExpression() =>
        Assert.Equal(Build().Tokens, Build().Tokens);

    private static (string[] Tokens, int Value) Build() =>
        EvaluateReversePolishNotationWorkloads.Build(OperandCount, new Random(Seed));

    private static bool IsOperand(string token) => token is not (Plus or Minus or Times or Divide);

    // Evaluates the tokens in 64-bit arithmetic, asserting along the way that every operator finds two
    // values, that no division is by zero, that every result fits a 32-bit int, and that exactly one
    // value is left at the end.
    private static long Replay(string[] tokens)
    {
        var values = new Stack<long>();

        foreach (var token in tokens)
        {
            if (IsOperand(token))
            {
                values.Push(long.Parse(token, CultureInfo.InvariantCulture));
                continue;
            }

            Assert.True(values.Count >= OperandsPerOperator);
            var right = values.Pop();
            var left = values.Pop();
            Assert.False(token == Divide && right == 0);
            var value = Apply(token, left, right);
            Assert.InRange(value, int.MinValue, int.MaxValue);
            values.Push(value);
        }

        return Assert.Single(values);
    }

    private static long Apply(string token, long left, long right) => token switch
    {
        Plus => left + right,
        Minus => left - right,
        Times => left * right,
        _ => left / right,
    };
}
