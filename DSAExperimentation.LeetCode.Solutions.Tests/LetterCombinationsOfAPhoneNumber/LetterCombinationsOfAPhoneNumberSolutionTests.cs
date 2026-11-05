using DSAExperimentation.LeetCode.Solutions.LetterCombinationsOfAPhoneNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LetterCombinationsOfAPhoneNumber;

// Harness only. Both strategies are LetterCombinationsOfAPhoneNumberSolution's;
// this file pins them to the empty input and to a single key. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class LetterCombinationsOfAPhoneNumberSolutionTests
{
    public static TheoryData<string, string[]> Cases =>
        new()
        {
            { "", [] },

            // A key other than 2 and 3, so letters beyond those keys' are pinned too.
            { "4", ["g", "h", "i"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LetterCombinationsByIterativeExpansion_Cases_ReturnsCartesianProductInPhoneOrder(
        string digits, string[] expected) =>
        Assert.Equal(
            expected,
            LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByIterativeExpansion(digits));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LetterCombinationsByBacktracking_Cases_ReturnsCartesianProductInPhoneOrder(
        string digits, string[] expected) =>
        Assert.Equal(
            expected,
            LetterCombinationsOfAPhoneNumberSolution.LetterCombinationsByBacktracking(digits));
}
