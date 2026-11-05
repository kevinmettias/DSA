using DSAExperimentation.LeetCode.Solutions.TheNumberOfWeakCharactersInTheGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TheNumberOfWeakCharactersInTheGame;

// Harness only. Both the pairwise comparison and the sort-then-scan pass are
// TheNumberOfWeakCharactersInTheGameSolution's - this file pins them to the ties
// LeetCode's examples never exercise: equal attacks with unequal defenses, wholly
// duplicate characters, and an equal-attack pair straddling a genuinely weak
// character, which is what the descending-attack/ascending-defense tie-break exists
// to get right - plus a lone character, a chain and a mixed board. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class TheNumberOfWeakCharactersInTheGameSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // A lone character has nobody to be weak against.
            { [[1, 1]], 0 },

            // Identical characters beat nobody: both axes need to be STRICTLY greater.
            { [[3, 3], [3, 3]], 0 },

            // Equal attack, unequal defense - the case a descending sort that broke ties
            // the other way would miscount as weak.
            { [[5, 1], [5, 2]], 0 },

            // A chain where each character is beaten by every later one.
            { [[1, 1], [2, 2], [3, 3]], 2 },

            // Two equal-attack characters, one of which beats the third on both axes.
            { [[7, 7], [1, 2], [7, 9]], 1 },

            // Mixed: only [7,1] is beaten on both (by [10,2]); [5,7] ties [7,7] on
            // defense and so survives.
            { [[10, 2], [5, 7], [7, 1], [7, 7]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWeakCharactersByPairwiseComparison_Cases_ReturnsWeakCharacterCount(
        int[][] properties, int expected) =>
        Assert.Equal(
            expected,
            TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersByPairwiseComparison(properties));

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfWeakCharactersBySortThenScan_Cases_ReturnsWeakCharacterCount(
        int[][] properties, int expected) =>
        Assert.Equal(
            expected,
            TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersBySortThenScan(properties));
}
