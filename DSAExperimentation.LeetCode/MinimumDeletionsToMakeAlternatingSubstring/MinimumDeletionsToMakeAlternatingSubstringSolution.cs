using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.MinimumDeletionsToMakeAlternatingSubstring;

// LeetCode 3777. Minimum Deletions to Make Alternating Substring: process a
// mixed stream of "flip s[j]" and "how many deletions does s[l..r] need to
// become alternating" queries over a string of 'A'/'B' characters.
//
// A substring is alternating iff no two adjacent characters are equal, so the
// deletions a range needs is exactly its count of adjacent-equal pairs -
// deleting one character from each such pair breaks it without ever creating
// a new one elsewhere. Modeling "edge j is an equal pair" (s[j] == s[j-1],
// for j in 1..n-1) as a 0/1 array turns every type-2 query into a range sum
// and every flip into at most two point updates (the flipped character only
// ever touches its two adjacent edges) - exactly the point-update/range-sum
// shape FenwickTree<Element, TOperation> already exists for.
internal static class MinimumDeletionsToMakeAlternatingSubstringSolution
{
    private const int FlipQuery = 1;

    // The textbook approach: mutate the character array in place on a flip,
    // re-scan the affected range on a query. O(1) per flip, O(r - l) per
    // query - the arm the Fenwick range-sum strategy below has to beat.
    public static int[] AnswerQueriesByDirectScan(string text, int[][] queries)
    {
        var chars = text.ToCharArray();
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == FlipQuery)
            {
                chars[query[1]] = Flip(chars[query[1]]);
            }
            else
            {
                var equalPairs = CountAdjacentEqualPairs(chars, query[1], query[2]);
                answers.Add(equalPairs);
            }
        }

        return answers.ToArray();
    }

    private static int CountAdjacentEqualPairs(char[] chars, int left, int right)
    {
        var count = 0;

        for (var i = left + 1; i <= right; i++)
        {
            if (chars[i] == chars[i - 1])
            {
                count++;
            }
        }

        return count;
    }

    // Each flip touches at most the two edges adjacent to the flipped index,
    // so every query - flip or range - is a single O(log n) Fenwick
    // operation, for O((n + q) log n) overall.
    public static int[] AnswerQueriesByFenwickAdjacency(string text, int[][] queries)
    {
        var chars = text.ToCharArray();
        var tree = new FenwickTree<int, SumOperation<int>>(BuildAdjacentEqualEdges(chars));
        var answers = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == FlipQuery)
            {
                Flip(tree, chars, query[1]);
            }
            else
            {
                var isSingleIndexRange = query[1] == query[2];
                answers.Add(isSingleIndexRange ? 0 : tree.Query(query[1], query[2] - 1));
            }
        }

        return answers.ToArray();
    }

    private static int[] BuildAdjacentEqualEdges(char[] chars)
    {
        var edges = new int[chars.Length - 1];

        for (var i = 1; i < chars.Length; i++)
        {
            edges[i - 1] = EdgeValue(chars[i - 1], chars[i]);
        }

        return edges;
    }

    private static void Flip(FenwickTree<int, SumOperation<int>> tree, char[] chars, int index)
    {
        var oldLeftEdge = index > 0 ? EdgeValue(chars[index - 1], chars[index]) : 0;
        var hasRightNeighbor = index < chars.Length - 1;
        var oldRightEdge = hasRightNeighbor ? EdgeValue(chars[index], chars[index + 1]) : 0;

        chars[index] = Flip(chars[index]);

        if (index > 0)
        {
            tree.Add(index - 1, EdgeValue(chars[index - 1], chars[index]) - oldLeftEdge);
        }

        if (index < chars.Length - 1)
        {
            tree.Add(index, EdgeValue(chars[index], chars[index + 1]) - oldRightEdge);
        }
    }

    private static int EdgeValue(char left, char right) => left == right ? 1 : 0;

    private static char Flip(char value) => value == 'A' ? 'B' : 'A';
}
