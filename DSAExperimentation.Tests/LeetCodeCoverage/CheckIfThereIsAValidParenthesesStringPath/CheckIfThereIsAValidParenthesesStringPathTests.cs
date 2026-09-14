using DSAExperimentation.LeetCode.CheckIfThereIsAValidParenthesesStringPath;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidParenthesesStringPath;

// Harness only. Both strategies are
// CheckIfThereIsAValidParenthesesStringPathSolution's, and pinning them to the same
// examples is what finally puts the un-memoized recursion - previously a private
// helper in the benchmark, asserted by nothing - under test alongside the memoized
// walk it is measured against.
public sealed class CheckIfThereIsAValidParenthesesStringPathTests
{
    public static TheoryData<char[,], bool> Examples =>
        new()
        {
            // LeetCode example 1: the accepting path runs along the top row and then
            // straight down the last column, spelling "((()))".
            {
                new[,]
                {
                    { '(', '(', '(' },
                    { ')', '(', ')' },
                    { '(', '(', ')' },
                    { '(', '(', ')' },
                },
                true
            },

            // LeetCode example 2: every path starts on ')', so the balance goes
            // negative on the very first cell.
            {
                new[,]
                {
                    { ')', ')' },
                    { '(', '(' },
                },
                false
            },

            // Right-then-down balances here: "(()" down to ")" closes out at zero.
            {
                new[,]
                {
                    { '(', '(', ')' },
                    { '(', ')', ')' },
                },
                true
            },

            // Same shape with a leading ')': the start cell alone is fatal.
            {
                new[,]
                {
                    { ')', '(', ')' },
                    { '(', ')', ')' },
                },
                false
            },

            // One cell can never balance - the path has odd length one.
            { new[,] { { '(' } }, false },

            // The smallest accepting grid: a single row spelling "()".
            { new[,] { { '(', ')' } }, true },

            // The same two cells reversed, so the only path opens with ')'.
            { new[,] { { ')', '(' } }, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByUnmemoizedRecursion_LeetCodeExamples_ReportsWhetherABalancedPathExists(
        char[,] grid, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByUnmemoizedRecursion(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByMemoizedRecursion_LeetCodeExamples_ReportsWhetherABalancedPathExists(
        char[,] grid, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfThereIsAValidParenthesesStringPathSolution.HasValidPathByMemoizedRecursion(grid));
}
