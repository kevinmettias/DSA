using DSAExperimentation.LeetCode.TheNumberOfWeakCharactersInTheGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfWeakCharactersInTheGame;

// Harness only. Both the pairwise comparison and the sort-then-scan pass are
// TheNumberOfWeakCharactersInTheGameSolution's - this file pins them to LeetCode's three
// published examples plus the ties the examples never exercise: equal attacks with
// unequal defenses, wholly duplicate characters, and an equal-attack pair straddling a
// genuinely weak character, which is what the descending-attack/ascending-defense
// tie-break exists to get right.
public sealed class TheNumberOfWeakCharactersInTheGameTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: no character beats another on both axes.
            { [[5, 5], [6, 3], [3, 6]], 0 },

            // LeetCode example 2: [3,3] beats [2,2] on both.
            { [[2, 2], [3, 3]], 1 },

            // LeetCode example 3: [10,4] beats [4,3] on both.
            { [[1, 5], [10, 4], [4, 3]], 1 },

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
    [MemberData(nameof(Examples))]
    public void NumberOfWeakCharactersByPairwiseComparison_LeetCodeExamples_ReturnsWeakCharacterCount(
        int[][] properties, int expected) =>
        Assert.Equal(
            expected,
            TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersByPairwiseComparison(properties));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWeakCharactersBySortThenScan_LeetCodeExamples_ReturnsWeakCharacterCount(
        int[][] properties, int expected) =>
        Assert.Equal(
            expected,
            TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersBySortThenScan(properties));
}
