using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

// LeetCode 3006. Find Beautiful Indices in the Given Array I: every index i where
// s[i..) starts with a and some occurrence of b in s sits within k positions of i.
//
// Both strategies reduce to the same two steps - find every occurrence of a and of
// b in s, then keep the a-occurrences that have a b-occurrence within k - and
// differ only in how the occurrences are found. #3008 asks the identical question
// at a scale this file's brute force cannot reach; the prefix-function strategy
// below is the one that also solves it.
internal static class FindBeautifulIndicesInTheGivenArrayISolution
{
    // The textbook double loop for both occurrence searches, then an O(|A| * |B|)
    // scan pairing every a-occurrence against every b-occurrence. Correct, and the
    // arm the composed strategy below has to beat.
    public static int[] FindBeautifulIndicesByBruteForce(SearchedText s, AnchorPattern a, NearbyPattern b, int k)
    {
        var aIndices = FindOccurrencesNaive(s, a.Text);
        var bIndices = FindOccurrencesNaive(s, b.Text);
        var result = new List<int>();

        foreach (var i in aIndices)
        {
            if (HasNearbyOccurrenceNaive(i, bIndices, k))
            {
                result.Add(i);
            }
        }

        return [.. result];
    }

    // Which of the two roles this pattern plays is the caller's business, so the search
    // itself takes only the text being scanned and the raw text to look for; `s` stays a
    // SearchedText so the two positions still cannot be handed over the wrong way round.
    private static List<int> FindOccurrencesNaive(SearchedText s, string pattern)
    {
        var matches = new List<int>();

        for (var i = 0; i + pattern.Length <= s.Text.Length; i++)
        {
            var isMatch = true;

            for (var j = 0; j < pattern.Length; j++)
            {
                if (s.Text[i + j] != pattern[j])
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                matches.Add(i);
            }
        }

        return matches;
    }

    private static bool HasNearbyOccurrenceNaive(int i, List<int> bIndices, int k)
    {
        foreach (var j in bIndices)
        {
            if (Math.Abs(i - j) <= k)
            {
                return true;
            }
        }

        return false;
    }

    // This repo's own KMP search (Algorithms.StringMatching.PrefixFunctionSearch)
    // finds every occurrence in O(s.Length + pattern.Length); pairing then walks
    // BinarySearch.LowerBound over the b-occurrences (already ascending, since
    // FindAll discovers them left to right) instead of scanning every b for every
    // a.
    public static int[] FindBeautifulIndicesByPrefixFunctionSearch(SearchedText s, AnchorPattern a, NearbyPattern b, int k)
    {
        var aIndices = PrefixFunctionSearch.FindAll(s.Text, a.Text);
        var bIndices = PrefixFunctionSearch.FindAll(s.Text, b.Text);

        return CollectNearbyIndices(aIndices, bIndices, k);
    }

    // aIndices and bIndices are both already ascending, so LowerBound(i - k)
    // locates the first b-occurrence that could possibly be within k of i in one
    // O(log |B|) probe instead of a linear scan of bIndices per a.
    private static int[] CollectNearbyIndices(List<int> aIndices, List<int> bIndices, int k)
    {
        var bSequence = new ArraySequence<int>([.. bIndices]);
        var result = new List<int>();

        foreach (var i in aIndices)
        {
            var lowerBound = BinarySearch.LowerBound(bSequence, i - k);

            if (lowerBound < bIndices.Count && bIndices[lowerBound] <= i + k)
            {
                result.Add(i);
            }
        }

        return [.. result];
    }

    // The three roles a beautiful-index query names, spelled out where a run of bare
    // `string` positions left them to the caller's memory. `s` is the text being
    // scanned, `a` the pattern an occurrence of which anchors a beautiful index, and
    // `b` the pattern that has to occur within k of it - different jobs, so different
    // types.
    internal readonly record struct SearchedText(string Text);

    internal readonly record struct AnchorPattern(string Text);

    internal readonly record struct NearbyPattern(string Text);
}
