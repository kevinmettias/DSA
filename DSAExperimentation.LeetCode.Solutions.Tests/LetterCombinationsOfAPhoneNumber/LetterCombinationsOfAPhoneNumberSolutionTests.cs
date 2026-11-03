using DSAExperimentation.LeetCode.Solutions.LetterCombinationsOfAPhoneNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LetterCombinationsOfAPhoneNumber;

// Harness only. Both strategies are LetterCombinationsOfAPhoneNumberSolution's;
// this file pins them to LeetCode's published examples.
public sealed partial class LetterCombinationsOfAPhoneNumberSolutionTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "23", ["ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf"] },
            { "2", ["a", "b", "c"] },
            { "", [] },

            // One key each, so every key's letters are pinned, not only 2's and 3's.
            { "4", ["g", "h", "i"] },
            { "5", ["j", "k", "l"] },
            { "6", ["m", "n", "o"] },
            { "7", ["p", "q", "r", "s"] },
            { "8", ["t", "u", "v"] },
            { "9", ["w", "x", "y", "z"] },
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
