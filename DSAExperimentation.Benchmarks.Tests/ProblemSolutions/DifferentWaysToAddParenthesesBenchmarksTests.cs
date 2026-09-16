using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DifferentWaysToAddParenthesesBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the same split/recurse/combine recurrence with and
// without this repo's Memoizer in front of it - so a harness whose arms disagree is timing two
// different problems. Setup builds "1+1+...+1" out of OperandCount copies of one operand, so the
// number of results each arm counts is the number of ways to fully parenthesize OperandCount
// operands: the (OperandCount - 1)th Catalan number, which is 42 for the six the benchmark's
// smallest parameter lists. Both arms report that same count from one fixed expression.
//
// A weakness worth stating plainly: both arms return `.Count` of the result list, so agreement
// witnesses that the two strategies produced the same NUMBER of results, not that they produced the
// same results. The expression repeats one operand, so every parenthesization of it evaluates to the
// same value anyway - the multiset of results is degenerate here by construction, which is why the
// count is the only thing left that can distinguish the arms. Strengthening this would mean changing
// a return type, which is a harness decision rather than this batch's.
public sealed partial class DifferentWaysToAddParenthesesBenchmarksTests
{
    private const int SmallestOperandCount = 6;
    private const int ExpectedParenthesizationsOfSixOnes = 42;

    [Fact]
    public void Setup_SixRepeatedOnes_CountsTheCatalanParenthesizationsAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedParenthesizationsOfSixOnes, harness.PlainRecursion());
        Assert.Equal(harness.PlainRecursion(), BuildHarness().PlainRecursion());
    }

    [Fact]
    public void PlainRecursion_RepeatedOperand_AgreesWithMemoizedSubstring()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedSubstring(), harness.PlainRecursion());
    }

    [Fact]
    public void MemoizedSubstring_RepeatedOperand_AgreesWithPlainRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PlainRecursion(), harness.MemoizedSubstring());
    }

    private static DifferentWaysToAddParenthesesBenchmarks BuildHarness()
    {
        var harness = new DifferentWaysToAddParenthesesBenchmarks { OperandCount = SmallestOperandCount };
        harness.Setup();

        return harness;
    }
}
