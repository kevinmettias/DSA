using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.DistinctEchoSubstrings;

// LeetCode 1316. Distinct Echo Substrings: an echo substring is text[start..start+2L)
// whose first half text[start..start+L) equals its second half
// text[start+L..start+2L); count how many *distinct* such substrings the text has.
//
// Both strategies sweep the same (start, halfLength) candidate space and dedupe by
// the echo's own text; they differ only in what one candidate's equality test costs.
internal static class DistinctEchoSubstringsSolution
{
    // An echo substring is exactly two equal-length halves back to back.
    private const int EchoHalfCount = 2;

    // The textbook answer: extract both halves as real substrings and compare them
    // with ==, paying O(halfLength) on every pair whether it matches or not, and
    // dedupe into a BCL HashSet<string>. Deliberately written without this repo's
    // primitives - it is the arm the composed strategy below has to beat.
    public static int CountDistinctEchoesByNaiveSubstringComparison(string text)
    {
        var echoes = new HashSet<string>();

        for (var halfLength = 1; halfLength * EchoHalfCount <= text.Length; halfLength++)
        {
            for (var start = 0; start + (EchoHalfCount * halfLength) <= text.Length; start++)
            {
                var first = text.Substring(start, halfLength);
                var second = text.Substring(start + halfLength, halfLength);

                if (first == second)
                {
                    var echo = text.Substring(start, halfLength * EchoHalfCount);
                    echoes.Add(echo);
                }
            }
        }

        return echoes.Count;
    }

    // This repo's own RollingHash as an O(1) equality screen per candidate pair,
    // only paying for a real SequenceEqual once the screen passes - the same
    // screen-then-verify shape RollingHashSearch.FindAll and LC 1147 use, since a
    // hash match is "probably equal", not "definitely equal". Survivors dedupe
    // through this repo's own Set<string> rather than a hand-rolled dictionary.
    public static int CountDistinctEchoesByRollingHashScreen(string text)
    {
        var hash = new RollingHash(text);
        var echoes = new Set<string>();

        for (var halfLength = 1; halfLength * EchoHalfCount <= text.Length; halfLength++)
        {
            for (var start = 0; start + (EchoHalfCount * halfLength) <= text.Length; start++)
            {
                if (IsEcho(text, hash, start, halfLength))
                {
                    var echo = text.Substring(start, halfLength * EchoHalfCount);
                    echoes.TryAdd(echo);
                }
            }
        }

        return echoes.Count;
    }

    private static bool IsEcho(string text, RollingHash hash, int start, int halfLength)
    {
        if (hash.Hash(start, halfLength) != hash.Hash(start + halfLength, halfLength))
        {
            return false;
        }

        var firstHalf = text.AsSpan(start, halfLength);
        var secondHalf = text.AsSpan(start + halfLength, halfLength);

        return firstHalf.SequenceEqual(secondHalf);
    }
}
