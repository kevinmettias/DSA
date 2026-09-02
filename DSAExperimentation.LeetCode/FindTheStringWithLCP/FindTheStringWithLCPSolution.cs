using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.FindTheStringWithLCP;

// LeetCode 2573. Find the String with LCP: lcp[i][j] is the longest common prefix of
// word[i:] and word[j:] for the hidden word, which for i != j collapses to exactly
// "word[i] == word[j]" (LCP(word[i:], word[j:]) > 0 iff their very first characters
// match). "Same character" is reflexive/symmetric/transitive - a genuine equivalence
// relation over positions [0, n) - which is exactly what this repo's own
// Collections.DisjointSet already models (Union-Find precedent). Both strategies group
// positions into character classes and assign each class the next available letter in
// index order (the greedy that produces the lexicographically smallest valid word),
// then reconstruct the LCP matrix the candidate actually has and compare it to the
// given one - the only way to catch an lcp input no real string could ever produce.
internal static class FindTheStringWithLCPSolution
{
    private const char FirstLetter = 'a';
    private const char LastLetter = 'z';

    // Baseline: the textbook direct sweep - for each not-yet-assigned position i, walk
    // j from i forward and stamp every lcp[i][j] > 0 position with i's own letter. No
    // repo primitive; just two nested loops and an array.
    public static string ConstructByDirectSweep(int[][] lcp)
    {
        var n = lcp.Length;
        var word = new char[n];
        var assigned = new bool[n];
        var nextLetter = FirstLetter;

        for (var i = 0; i < n; i++)
        {
            if (assigned[i])
            {
                continue;
            }

            if (nextLetter > LastLetter)
            {
                return string.Empty;
            }

            for (var j = i; j < n; j++)
            {
                if (lcp[i][j] > 0)
                {
                    word[j] = nextLetter;
                    assigned[j] = true;
                }
            }

            nextLetter++;
        }

        return IsConsistent(word, lcp) ? new string(word) : string.Empty;
    }

    // Composed: DisjointSet unions every pair of positions with a nonzero LCP into one
    // character class, then each class's root is mapped to the next available letter
    // in the order its first member appears.
    public static string ConstructByDisjointSet(int[][] lcp)
    {
        var n = lcp.Length;
        var positions = new DisjointSet(n);

        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                if (lcp[i][j] > 0)
                {
                    positions.Union(i, j);
                }
            }
        }

        var word = new char[n];
        var letterForRoot = new Dictionary<int, char>();
        var nextLetter = FirstLetter;

        for (var i = 0; i < n; i++)
        {
            var root = positions.Find(i);

            if (!letterForRoot.TryGetValue(root, out var letter))
            {
                if (nextLetter > LastLetter)
                {
                    return string.Empty;
                }

                letter = nextLetter++;
                letterForRoot[root] = letter;
            }

            word[i] = letter;
        }

        return IsConsistent(word, lcp) ? new string(word) : string.Empty;
    }

    // Rebuilds the LCP matrix implied by `candidate` from the bottom-right corner
    // upward (lcp[i][j] = lcp[i+1][j+1] + 1 when the characters match, else 0) and
    // compares it cell-by-cell against the given matrix.
    private static bool IsConsistent(char[] candidate, int[][] lcp)
    {
        var n = candidate.Length;
        var actual = new int[n + 1, n + 1];

        for (var i = n - 1; i >= 0; i--)
        {
            for (var j = n - 1; j >= 0; j--)
            {
                actual[i, j] = candidate[i] == candidate[j] ? actual[i + 1, j + 1] + 1 : 0;

                if (actual[i, j] != lcp[i][j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
