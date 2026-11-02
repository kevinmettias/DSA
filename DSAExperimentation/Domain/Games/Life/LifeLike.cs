namespace DSAExperimentation.Domain.Games.Life;

// Whether a cell is live in the next generation under a Life-like rule, asked one way for a dead cell
// and another for a live one, so a caller that knows its cell's state picks one rather than passing
// it in as a flag. The rule's masks are static abstract members of a struct, so each rule's test
// folds to a shift and a mask of constants. A count is the number of live neighbours, 0 through 8;
// the board's encoding of live and dead, and which cells count as neighbours, stay with each problem.
/// <summary>The next-generation rule of a Life-like automaton.</summary>
/// <typeparam name="TRule">The rule, such as <see cref="ConwayLife"/>.</typeparam>
public static class LifeLike<TRule>
    where TRule : struct, ILifeLikeRule
{
    /// <summary>Whether a dead cell with this many live neighbours is born.</summary>
    /// <param name="liveNeighbors">How many of the cell's neighbours are live, 0 through 8.</param>
    /// <returns><see langword="true"/> when the rule's birth mask holds the count; otherwise <see langword="false"/>.</returns>
    public static bool IsBirthCount(int liveNeighbors) => ((TRule.BirthMask >> liveNeighbors) & 1) != 0;

    /// <summary>Whether a live cell with this many live neighbours survives.</summary>
    /// <param name="liveNeighbors">How many of the cell's neighbours are live, 0 through 8.</param>
    /// <returns><see langword="true"/> when the rule's survival mask holds the count; otherwise <see langword="false"/>.</returns>
    public static bool IsSurvivalCount(int liveNeighbors) => ((TRule.SurvivalMask >> liveNeighbors) & 1) != 0;
}
