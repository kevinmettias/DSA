using DSAExperimentation.LeetCode.CheckIfThereIsAValidParenthesesStringPath;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidParenthesesStringPath;

// Harness only. Both strategies are
// CheckIfThereIsAValidParenthesesStringPathSolution's, and pinning them to the same
// examples is what finally puts the un-memoized recursion - previously a private
// helper in the benchmark, asserted by nothing - under test alongside the memoized
// walk it is measured against.
public sealed class CheckIfThereIsAValidParenthesesStringPathTests
{
    public static TheoryData<ParenthesesGridCase> Examples =>
        new()
        {
            // LeetCode example 1: the accepting path runs along the top row and then
            // straight down the last column, spelling "((()))".
            {
                new ParenthesesGridCase(
                    new[,]
                    {
                        { '(', '(', '(' },
                        { ')', '(', ')' },
                        { '(', '(', ')' },
                        { '(', '(', ')' },
                    },
                    Expected: true)
            },

            // LeetCode example 2: every path starts on ')', so the balance goes
            // negative on the very first cell.
            {
                new ParenthesesGridCase(
                    new[,]
                    {
                        { ')', ')' },
                        { '(', '(' },
                    },
                    Expected: false)
            },

            // Right-then-down balances here: "(()" down to ")" closes out at zero.
            {
                new ParenthesesGridCase(
                    new[,]
                    {
                        { '(', '(', ')' },
                        { '(', ')', ')' },
                    },
                    Expected: true)
            },

            // Same shape with a leading ')': the start cell alone is fatal.
            {
                new ParenthesesGridCase(
                    new[,]
                    {
                        { ')', '(', ')' },
                        { '(', ')', ')' },
                    },
                    Expected: false)
            },

            // One cell can never balance - the path has odd length one.
            { new ParenthesesGridCase(new[,] { { '(' } }, Expected: false) },

            // The smallest accepting grid: a single row spelling "()".
            { new ParenthesesGridCase(new[,] { { '(', ')' } }, Expected: true) },

            // The same two cells reversed, so the only path opens with ')'.
            { new ParenthesesGridCase(new[,] { { ')', '(' } }, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByUnmemoizedRecursion_LeetCodeExamples_ReportsWhetherABalancedPathExists(
        ParenthesesGridCase example) =>
        Assert.Equal(
            example.Expected,
            CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByUnmemoizedRecursion(example.Grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByMemoizedRecursion_LeetCodeExamples_ReportsWhetherABalancedPathExists(
        ParenthesesGridCase example) =>
        Assert.Equal(
            example.Expected,
            CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByMemoizedRecursion(example.Grid));

    // One LeetCode example: the parenthesis grid and whether a balanced path through
    // it exists. The expected value is named at every construction site, so a row
    // reads as the case it is rather than as a bare `true` whose meaning is its
    // position. Nested because it is only ever used inside this test class - it is
    // this harness's own vocabulary, not a type another file would import.
    public readonly record struct ParenthesesGridCase(char[,] Grid, bool Expected);
}
