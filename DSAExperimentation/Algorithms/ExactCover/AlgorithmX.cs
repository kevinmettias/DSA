using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Algorithms.ExactCover;

// Knuth's Algorithm X (2000): to cover the primary columns still uncovered, take the one the fewest
// rows meet, and try each of those rows in turn: choose it, cover every column it meets, which
// takes out every row clashing with it, and solve what is left; then undo it and try the next.
// When no primary column is left the rows chosen are a cover. Taking the column with the fewest
// rows branches least, and fails at once on a column no row can meet any more. The matrix is
// DancingLinks', which undoes each cover in the order it was made, so the search keeps one matrix
// and copies nothing. Rows chosen in advance, such as a puzzle's givens, are covered before the
// search, and start every cover it reports.
//
// The problem is read once, into the links, so its rows compile into the build alone; the search
// runs on flat arrays. What happens to each cover is a visitor struct passed by reference: keep
// the first, count them all, or the caller's own. The recursion goes one level per row chosen, and
// each level covers at least one primary column, so it is no deeper than the primary columns are
// many.
/// <summary>Finds exact covers by Knuth's Algorithm X over dancing links.</summary>
public static class AlgorithmX
{
    /// <summary>Finds one exact cover of a problem.</summary>
    /// <typeparam name="TProblem">The problem.</typeparam>
    /// <param name="problem">The problem, carrying its input.</param>
    /// <param name="chosenRows">Rows the cover must contain, such as a puzzle's givens.</param>
    /// <param name="cover">
    /// The cover found, <paramref name="chosenRows"/> first; empty when there is none.
    /// </param>
    /// <returns><see langword="true"/> when a cover holds the chosen rows.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A chosen row is not one of the problem's, or a row meets a column the problem does not have.
    /// </exception>
    /// <exception cref="ArgumentException">A row meets one column twice.</exception>
    public static bool TryFindCover<TProblem>(
        TProblem problem, ReadOnlySpan<int> chosenRows, out int[] cover)
        where TProblem : struct, IExactCoverProblem
    {
        var first = new FirstCover();
        var isFound = TrySearch(problem, chosenRows, ref first);
        cover = first.Rows;

        return isFound;
    }

    /// <summary>Counts the exact covers of a problem.</summary>
    /// <typeparam name="TProblem">The problem.</typeparam>
    /// <param name="problem">The problem, carrying its input.</param>
    /// <param name="chosenRows">Rows every cover counted must contain.</param>
    /// <returns>How many covers hold the chosen rows.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A chosen row is not one of the problem's, or a row meets a column the problem does not have.
    /// </exception>
    /// <exception cref="ArgumentException">A row meets one column twice.</exception>
    public static long CountCovers<TProblem>(TProblem problem, ReadOnlySpan<int> chosenRows)
        where TProblem : struct, IExactCoverProblem
    {
        var count = new CoverCount();
        TrySearch(problem, chosenRows, ref count);

        return count.Count;
    }

    /// <summary>
    /// Hands each exact cover of a problem to a visitor, until the visitor stops the search.
    /// </summary>
    /// <typeparam name="TProblem">The problem.</typeparam>
    /// <typeparam name="TVisitor">What to do with each cover.</typeparam>
    /// <param name="problem">The problem, carrying its input.</param>
    /// <param name="chosenRows">Rows every cover must contain.</param>
    /// <param name="visitor">The visitor, which keeps what it records.</param>
    /// <returns>
    /// <see langword="true"/> when the visitor stopped the search; <see langword="false"/> when
    /// every cover was handed over, or the chosen rows clash.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A chosen row is not one of the problem's, or a row meets a column the problem does not have.
    /// </exception>
    /// <exception cref="ArgumentException">A row meets one column twice.</exception>
    public static bool TrySearch<TProblem, TVisitor>(
        TProblem problem, ReadOnlySpan<int> chosenRows, ref TVisitor visitor)
        where TProblem : struct, IExactCoverProblem
        where TVisitor : struct, ICoverVisitor
    {
        var links = DancingLinks.Of(problem);

        if (!links.TryChooseAll(chosenRows))
        {
            return false;
        }

        var cover = new int[chosenRows.Length + problem.PrimaryColumnCount];
        chosenRows.CopyTo(cover);
        var search = new CoverSearch(links, cover);

        return search.TrySearchFrom(chosenRows.Length, ref visitor);
    }

    // One search: the links, and the rows chosen so far, cover[..depth] at each level. On a stop
    // the links are left as they stand, since nothing searches them again.
    private sealed class CoverSearch(DancingLinks links, int[] cover)
    {
        // One level: a cover when nothing is left to cover, or else each row meeting the column
        // fewest rows meet, tried in turn.
        public bool TrySearchFrom<TVisitor>(int depth, ref TVisitor visitor)
            where TVisitor : struct, ICoverVisitor
        {
            if (links.IsComplete)
            {
                var found = cover.AsSpan(0, depth);

                return visitor.ShouldStopAfter(found);
            }

            var column = links.ColumnWithFewestRows();
            links.Cover(column);

            if (TryEachRow(column, depth, ref visitor))
            {
                return true;
            }

            links.Uncover(column);

            return false;
        }

        // Each row meeting a covered column, chosen in turn: its other columns covered, the rest
        // searched, and the row unchosen again unless the visitor stopped the search.
        private bool TryEachRow<TVisitor>(int column, int depth, ref TVisitor visitor)
            where TVisitor : struct, ICoverVisitor
        {
            for (var node = links.Below(column); node != column; node = links.Below(node))
            {
                cover[depth] = links.RowOf(node);
                links.CoverRestOfRow(node);

                if (TrySearchFrom(depth + 1, ref visitor))
                {
                    return true;
                }

                links.UncoverRestOfRow(node);
            }

            return false;
        }
    }
}
