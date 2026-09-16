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
        var state = (Lcp: lcp, Word: word, Assigned: assigned);
        var nextLetter = FirstLetter;

        for (var i = 0; i < n; i++)
        {
            if (assigned[i])
            {
                continue;
            }

            if (!TryStampClassWithLetter(state, i, nextLetter))
            {
                return string.Empty;
            }

            nextLetter++;
        }

        return IsConsistent(word, lcp) ? new string(word) : string.Empty;
    }

    // Stamps the letter of the class at `positionIndex` across every later position that
    // shares it, and reports false once the alphabet is exhausted - no valid word can
    // then exist.
    private static bool TryStampClassWithLetter(
        (int[][] Lcp, char[] Word, bool[] Assigned) state, int positionIndex, char letter)
    {
        if (letter > LastLetter)
        {
            return false;
        }

        for (var j = positionIndex; j < state.Lcp.Length; j++)
        {
            if (state.Lcp[positionIndex][j] > 0)
            {
                state.Word[j] = letter;
                state.Assigned[j] = true;
            }
        }

        return true;
    }

    // Composed: DisjointSet unions every pair of positions with a nonzero LCP into one
    // character class, then each class's root is mapped to the next available letter
    // in the order its first member appears.
    public static string ConstructByDisjointSet(int[][] lcp)
    {
        var positions = BuildCharacterClasses(lcp);
        var word = new char[lcp.Length];

        if (!TryFillLettersByClass(positions, word))
        {
            return string.Empty;
        }

        return IsConsistent(word, lcp) ? new string(word) : string.Empty;
    }

    // Unions every pair of positions with a nonzero LCP into one character class.
    private static DisjointSet BuildCharacterClasses(int[][] lcp)
    {
        var positions = new DisjointSet(lcp.Length);

        for (var i = 0; i < lcp.Length; i++)
        {
            for (var j = i + 1; j < lcp.Length; j++)
            {
                if (lcp[i][j] > 0)
                {
                    positions.Union(i, j);
                }
            }
        }

        return positions;
    }

    // Fills each position with its class's letter, minting the letters in the order each
    // class's first member appears. Reports false once the alphabet runs out.
    private static bool TryFillLettersByClass(DisjointSet positions, char[] word)
    {
        var letterForRoot = new Dictionary<int, char>();
        var nextLetter = FirstLetter;

        for (var i = 0; i < word.Length; i++)
        {
            var root = positions.Find(i);

            if (!letterForRoot.TryGetValue(root, out var letter))
            {
                if (nextLetter > LastLetter)
                {
                    return false;
                }

                letter = nextLetter++;
                letterForRoot[root] = letter;
            }

            word[i] = letter;
        }

        return true;
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
                var charactersMatch = candidate[i] == candidate[j];
                actual[i, j] = charactersMatch ? MatchedLcp(actual, i, j) : 0;

                if (actual[i, j] != lcp[i][j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    // The characters match, so this pair's lcp is the next pair's, one longer.
    private static int MatchedLcp(int[,] actual, int firstIndex, int secondIndex) =>
        actual[firstIndex + 1, secondIndex + 1] + 1;
}
