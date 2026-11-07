using System.Text;

namespace DSAExperimentation.Domain.Games.Zuma;

// A Zuma-style row's collapse: a group of touching balls of one colour at least the rule's length
// vanishes, the balls either side then touch, and a group those form vanishes in turn, until none is
// left. This is the game's *what* (ARCHITECTURE §17.6). The colour letters are LeetCode's spelling,
// and the collapse only asks whether two balls match, so no colour is named here. How a strategy
// searches its insertions stays with it, and both of LC 488's arms collapse through this (§17.5).
//
// Each pass removes every vanishing group at once, and passes repeat until one changes nothing. That
// is the game's one-group-at-a-time cascade only while the row holds at most one such group to begin
// with, as it does right after one ball goes into a settled row, the only way LC 488 collapses. The
// pass is LC 488's own, moved unchanged, so measuring the move measures only it.
/// <summary>The collapse of touching same-coloured balls in a Zuma-style row.</summary>
/// <typeparam name="TRule">The rule saying how many touching balls vanish, such as <see cref="ClassicZuma"/>.</typeparam>
public static class ZumaRow<TRule>
    where TRule : struct, IZumaRule
{
    /// <summary>The row left once every vanishing group has vanished, cascading.</summary>
    /// <param name="row">The balls in order, one character per colour, holding at most one vanishing group.</param>
    /// <returns>The settled row, which holds no vanishing group.</returns>
    public static string Collapse(string row)
    {
        // Stops at the first pass that changes nothing: no vanishing group remains.
        while (true)
        {
            var next = CollapseOnePass(row);

            if (next == row)
            {
                return next;
            }

            row = next;
        }
    }

    private static string CollapseOnePass(string row)
    {
        var builder = new StringBuilder();
        var i = 0;

        while (i < row.Length)
        {
            var j = i;

            while (j < row.Length && row[j] == row[i])
            {
                j++;
            }

            if (j - i < TRule.ShortestVanishingGroup)
            {
                builder.Append(row, i, j - i);
            }

            i = j;
        }

        return builder.ToString();
    }
}
