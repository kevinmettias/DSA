using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// Token graphs whose every position's value is known, for the retrograde solver's tests.
internal static class TokenGraphs
{
    // Drawn and solved by hand under normal play: 0 -> 1 -> 2 -> 0, a cycle with no way out; 3 with
    // no edge out; 4 -> 3; 5 -> 4; 6 -> 5 or 0; 7 -> 4 or 0; 8 -> 7 or 4; 9 -> itself or 4; and
    // 10 -> 11, 11 -> 10 or 3, a cycle with a way out.
    public static TokenGraphGame Loopy { get; } = new(
        [[1], [2], [0], [], [3], [4], [5, 0], [4, 0], [7, 4], [9, 4], [11], [10, 3]],
        GameOutcome.Loss);

    // A take-away game as a token graph under normal play: vertex n is a pile of n stones, with an
    // edge to each pile a turn taking one to `most` stones leaves. It never returns to a pile.
    public static TokenGraphGame TakeAway(int most, int largestPile)
    {
        var edges = new int[largestPile + 1][];

        for (var pile = 0; pile <= largestPile; pile++)
        {
            var moveCount = Math.Min(pile, most);
            edges[pile] = [.. Enumerable.Range(1, moveCount).Select(taken => pile - taken)];
        }

        return new TokenGraphGame(edges, GameOutcome.Loss);
    }

    // A path 0 -> 1 -> ... -> length - 1, whose last vertex has no edge out.
    public static TokenGraphGame Path(int length, GameOutcome whenStuck)
    {
        var edges = new int[length][];

        for (var vertex = 0; vertex < length - 1; vertex++)
        {
            edges[vertex] = [vertex + 1];
        }

        edges[length - 1] = [];

        return new TokenGraphGame(edges, whenStuck);
    }
}
