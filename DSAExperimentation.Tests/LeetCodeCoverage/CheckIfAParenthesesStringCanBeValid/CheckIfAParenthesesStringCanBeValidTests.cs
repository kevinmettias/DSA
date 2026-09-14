using DSAExperimentation.LeetCode.CheckIfAParenthesesStringCanBeValid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAParenthesesStringCanBeValid;

// Harness only: both strategies live in
// CheckIfAParenthesesStringCanBeValidSolution and are asserted against the same
// examples - LeetCode's three published ones plus the odd-length free position that
// the benchmark's stack arm used to get wrong, a free position that has to become
// '(' to save a locked ')', and a free position sitting before a locked '(' that no
// rewriting can rescue.
public sealed class CheckIfAParenthesesStringCanBeValidTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "))()))", "010100", true },  // LeetCode example 1
            { "()()", "0000", true },      // LeetCode example 2
            { ")", "0", false },           // LeetCode example 3
            { "(", "0", false },           // odd length: a free position must still become a bracket
            { "()", "11", true },          // fully locked and already balanced
            { ")(", "11", false },         // fully locked and unbalanceable
            { "))", "01", true },          // the leading free position becomes '('
            { "))", "10", false },         // the leading locked ')' has nothing to match
            { ")(", "01", false },         // the free position sits before the locked '('
            { "(())", "1111", true },      // nested and fully locked
            { "(((", "000", false },       // odd length, everything free
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeValidByReachableOpenCountDp_LeetCodeExamples_ReturnsExpectedValidity(
        string s, string locked, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByReachableOpenCountDp(s, locked));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeValidByIndexStackSweep_LeetCodeExamples_ReturnsExpectedValidity(
        string s, string locked, bool expected) =>
        Assert.Equal(
            expected,
            CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByIndexStackSweep(s, locked));
}
