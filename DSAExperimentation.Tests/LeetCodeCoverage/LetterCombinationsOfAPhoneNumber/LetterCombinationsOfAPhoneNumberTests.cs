using DSAExperimentation.LeetCode.LetterCombinationsOfAPhoneNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LetterCombinationsOfAPhoneNumber;

// Harness only. Both strategies are LetterCombinationsOfAPhoneNumberSolution's;
// this file pins them to LeetCode's published examples.
public sealed class LetterCombinationsOfAPhoneNumberTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "23", ["ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf"] },
            { "2", ["a", "b", "c"] },
            { "", [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LetterCombinationsByIterativeExpansion_LeetCodeExamples_ReturnsCartesianProductInPhoneOrder(
        string digits, string[] expected) =>
        Assert.Equal(
            expected,
            LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByIterativeExpansion(digits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LetterCombinationsByBacktracking_LeetCodeExamples_ReturnsCartesianProductInPhoneOrder(
        string digits, string[] expected) =>
        Assert.Equal(
            expected,
            LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByBacktracking(digits));
}
