using DSAExperimentation.LeetCode.CheckIfAParenthesesStringCanBeValid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAParenthesesStringCanBeValid;

// Harness only: both strategies live in
// CheckIfAParenthesesStringCanBeValidSolution and are asserted against the same
// examples - LeetCode's three published ones plus the odd-length free position that
// the benchmark's stack arm used to get wrong, a free position that has to become
// '(' to save a locked ')', and a free position sitting before a locked '(' that no
// rewriting can rescue.
public sealed partial class CheckIfAParenthesesStringCanBeValidTests
{
    public static TheoryData<ValidityCase> Examples =>
        new()
        {
            // LeetCode example 1
            { new ValidityCase(S: "))()))", Locked: "010100", Expected: true) },
            // LeetCode example 2
            { new ValidityCase(S: "()()", Locked: "0000", Expected: true) },
            // LeetCode example 3
            { new ValidityCase(S: ")", Locked: "0", Expected: false) },
            // odd length: a free position must still become a bracket
            { new ValidityCase(S: "(", Locked: "0", Expected: false) },
            // fully locked and already balanced
            { new ValidityCase(S: "()", Locked: "11", Expected: true) },
            // fully locked and unbalanceable
            { new ValidityCase(S: ")(", Locked: "11", Expected: false) },
            // the leading free position becomes '('
            { new ValidityCase(S: "))", Locked: "01", Expected: true) },
            // the leading locked ')' has nothing to match
            { new ValidityCase(S: "))", Locked: "10", Expected: false) },
            // the free position sits before the locked '('
            { new ValidityCase(S: ")(", Locked: "01", Expected: false) },
            // nested and fully locked
            { new ValidityCase(S: "(())", Locked: "1111", Expected: true) },
            // odd length, everything free
            { new ValidityCase(S: "(((", Locked: "000", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeValidByReachableOpenCountDp_LeetCodeExamples_ReturnsExpectedValidity(
        ValidityCase example)
    {
        var valid = CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByReachableOpenCountDp(
            new CheckIfAParenthesesStringCanBeValidSolution.ParenthesisString(example.S),
            new CheckIfAParenthesesStringCanBeValidSolution.LockMask(example.Locked));

        Assert.Equal(example.Expected, valid);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBeValidByIndexStackSweep_LeetCodeExamples_ReturnsExpectedValidity(
        ValidityCase example)
    {
        var valid = CheckIfAParenthesesStringCanBeValidSolution.CanBeValidByIndexStackSweep(
            new CheckIfAParenthesesStringCanBeValidSolution.ParenthesisString(example.S),
            new CheckIfAParenthesesStringCanBeValidSolution.LockMask(example.Locked));

        Assert.Equal(example.Expected, valid);
    }

    // One LeetCode example: the bracket string, its lock mask, and whether the two can
    // be made into a balanced string. The two strings are the same type and the relation
    // between them is not symmetric, so the row names which is which rather than leaving
    // two interchangeable positions. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct ValidityCase(string S, string Locked, bool Expected);
}
