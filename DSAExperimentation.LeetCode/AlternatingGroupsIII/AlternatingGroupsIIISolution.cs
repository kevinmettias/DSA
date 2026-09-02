namespace DSAExperimentation.LeetCode.AlternatingGroupsIII;

// LeetCode 3245. Alternating Groups III: tiles are colored 0/1 around a circle.
// queries[i] = [1, size] asks how many of the circle's own contiguous windows of
// that many tiles alternate color throughout ("size" is a shorthand for both the
// window's tile count and, per the problem's wording, an alternating group of
// exactly that size); queries[i] = [2, index, color] repaints one tile.
//
// Every strategy here answers the same "process every query in order, report the
// type-1 answers" shape - the difference is entirely in how a size query is
// answered.
internal static class AlternatingGroupsIIISolution
{
    private const int CountQuery = 1;

    // Textbook baseline: a size query walks every one of the n circular starting
    // positions and checks its window directly, stopping at the first repeated
    // neighbor; an update is a plain array write. O(n) per size query - the arm
    // AlternatingRunLedger's O(log n) queries have to beat - deliberately no
    // repo primitive beyond the BCL array LeetCode's own input already is.
    public static IList<int> NumberOfAlternatingGroupsByBruteForce(int[] colors, int[][] queries)
    {
        var tiles = (int[])colors.Clone();
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == CountQuery)
            {
                answers.Add(CountAlternatingWindows(tiles, query[1]));
            }
            else
            {
                tiles[query[1]] = query[2];
            }
        }

        return answers;
    }

    private static int CountAlternatingWindows(int[] tiles, int size)
    {
        var tileCount = tiles.Length;
        var count = 0;

        for (var start = 0; start < tileCount; start++)
        {
            var isAlternating = true;

            for (var offset = 1; offset < size && isAlternating; offset++)
            {
                if (tiles[(start + offset) % tileCount] == tiles[(start + offset - 1) % tileCount])
                {
                    isAlternating = false;
                }
            }

            if (isAlternating)
            {
                count++;
            }
        }

        return count;
    }

    // AlternatingRunLedger keeps every maximal alternating run's length in two
    // FenwickTrees (count-by-length and length-sum-by-length), so a size query
    // is two prefix lookups (O(log n)) instead of a full circle scan, and a
    // repaint touches at most two edges (O(log n) to locate, O(tileCount) worst
    // case to shift the sorted bad-edge DynamicArray - see its own doc comment).
    public static IList<int> NumberOfAlternatingGroupsByRunLengthFenwick(int[] colors, int[][] queries)
    {
        var runs = new AlternatingRunLedger(colors);
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == CountQuery)
            {
                answers.Add(runs.CountWindows(query[1]));
            }
            else
            {
                runs.Repaint(query[1], query[2]);
            }
        }

        return answers;
    }
}
