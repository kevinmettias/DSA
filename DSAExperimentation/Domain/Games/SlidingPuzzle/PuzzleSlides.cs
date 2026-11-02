using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

// A sliding puzzle as a state space, for StateGraph: every arrangement of the board's symbols is a
// state, numbered in the order the arrangements read, and a move slides one tile into the empty
// cell. The space has CellCount! states, so only small boards can be enumerated: 2 by 3 has 720 and
// 3 by 3 has 362,880, while 13 cells or more overflow the count, and StateCount throws.
/// <summary>A sliding puzzle's arrangements as a state space, each one slide from the next.</summary>
/// <typeparam name="TBoard">The board's shape.</typeparam>
public readonly struct PuzzleSlides<TBoard> : IStateSpace<string>
    where TBoard : struct, IPuzzleBoard
{
    /// <summary>Every arrangement of the board's cells: their count, factorial.</summary>
    /// <exception cref="OverflowException">The board has more than 12 cells.</exception>
    public int StateCount => Arrangements(SlidingPuzzle<TBoard>.CellCount);

    /// <summary>The most slides from any arrangement: one per neighbour of the empty cell.</summary>
    public int MaxSuccessors => SlidingPuzzle<TBoard>.MostSlides;

    /// <summary>Spells an arrangement by its number, arrangements numbered in the order they read.</summary>
    /// <param name="index">The arrangement's number, in [0, <see cref="StateCount"/>).</param>
    /// <returns>The arrangement, spelled row by row.</returns>
    public string StateAt(int index)
    {
        var unplaced = SlidingPuzzle<TBoard>.CellSymbols.ToList();

        return string.Create(unplaced.Count, (unplaced, index), static (cells, numbering) =>
        {
            var (left, rest) = numbering;

            for (var cell = 0; cell < cells.Length; cell++)
            {
                var block = Arrangements(left.Count - 1);
                cells[cell] = left[rest / block];
                left.RemoveAt(rest / block);
                rest %= block;
            }
        });
    }

    private static int Arrangements(int cells)
    {
        var count = 1;

        for (var cell = 1; cell <= cells; cell++)
        {
            count = checked(count * cell);
        }

        return count;
    }

    /// <summary>Writes the arrangements one slide away.</summary>
    /// <param name="state">The arrangement slid from.</param>
    /// <param name="successors">Where to write them.</param>
    /// <returns>How many were written.</returns>
    public int WriteSuccessors(string state, Span<string> successors) => SlidingPuzzle<TBoard>.WriteOneSlideAway(state, successors);
}
