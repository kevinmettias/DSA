using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

// LeetCode 3006. Find Beautiful Indices in the Given Array I: every start index where
// the searched text from there starts with the anchor pattern and some occurrence of
// the nearby pattern sits within maxDistance positions of it.
//
// Both strategies reduce to the same two steps - find every occurrence of the anchor
// pattern and of the nearby pattern in the searched text, then keep the anchor
// occurrences that have a nearby occurrence within maxDistance - and differ only in
// how the occurrences are found. #3008 asks the identical question at a scale this
// file's brute force cannot reach; the prefix-function strategy below is the one that
// also solves it.
internal static class FindBeautifulIndicesInTheGivenArrayISolution
{
    // The textbook double loop for both occurrence searches, then an O(|A| * |B|)
    // scan pairing every anchor occurrence against every nearby occurrence. Correct,
    // and the arm the composed strategy below has to beat.
    public static int[] FindBeautifulIndicesByBruteForce(
        SearchedText searchedText, AnchorPattern anchorPattern, NearbyPattern nearbyPattern, int maxDistance)
    {
        var aIndices = FindOccurrencesNaive(searchedText, anchorPattern.Text);
        var bIndices = FindOccurrencesNaive(searchedText, nearbyPattern.Text);
        var result = new List<int>();

        foreach (var i in aIndices)
        {
            if (HasNearbyOccurrenceNaive(i, bIndices, maxDistance))
            {
                result.Add(i);
            }
        }

        return [.. result];
    }

    // Which of the two roles this pattern plays is the caller's business, so the search
    // itself takes only the text being scanned and the raw text to look for;
    // `searchedText` stays a SearchedText so the two positions still cannot be handed
    // over the wrong way round.
    private static List<int> FindOccurrencesNaive(SearchedText searchedText, string pattern)
    {
        var matches = new List<int>();

        for (var i = 0; i + pattern.Length <= searchedText.Text.Length; i++)
        {
            var isMatch = true;

            for (var j = 0; j < pattern.Length; j++)
            {
                if (searchedText.Text[i + j] != pattern[j])
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

    private static bool HasNearbyOccurrenceNaive(int index, List<int> bIndices, int maxDistance)
    {
        foreach (var j in bIndices)
        {
            if (Math.Abs(index - j) <= maxDistance)
            {
                return true;
            }
        }

        return false;
    }

    // This repo's own KMP search (Algorithms.StringMatching.PrefixFunctionSearch)
    // finds every occurrence in O(searchedText.Length + pattern.Length); pairing then
    // walks BinarySearch.LowerBound over the nearby occurrences (already ascending,
    // since FindAll discovers them left to right) instead of scanning every nearby
    // occurrence for every anchor occurrence.
    public static int[] FindBeautifulIndicesByPrefixFunctionSearch(
        SearchedText searchedText, AnchorPattern anchorPattern, NearbyPattern nearbyPattern, int maxDistance)
    {
        var aIndices = PrefixFunctionSearch.FindAll(searchedText.Text, anchorPattern.Text);
        var bIndices = PrefixFunctionSearch.FindAll(searchedText.Text, nearbyPattern.Text);

        return CollectNearbyIndices(aIndices, bIndices, maxDistance);
    }

    // aIndices and bIndices are both already ascending, so LowerBound(index - maxDistance)
    // locates the first nearby occurrence that could possibly be within maxDistance of
    // index in one O(log |B|) probe instead of a linear scan of bIndices per anchor index.
    private static int[] CollectNearbyIndices(List<int> aIndices, List<int> bIndices, int maxDistance)
    {
        var bSequence = new ArraySequence<int>([.. bIndices]);
        var result = new List<int>();

        foreach (var i in aIndices)
        {
            var lowerBound = BinarySearch.LowerBound(bSequence, i - maxDistance);

            if (lowerBound < bIndices.Count && bIndices[lowerBound] <= i + maxDistance)
            {
                result.Add(i);
            }
        }

        return [.. result];
    }

    // The three roles a beautiful-index query names, spelled out where a run of bare
    // `string` positions left them to the caller's memory. `searchedText` is the text
    // being scanned, `anchorPattern` the pattern an occurrence of which anchors a
    // beautiful index, and `nearbyPattern` the pattern that has to occur within
    // `maxDistance` of it - different jobs, so different types.
    internal readonly record struct SearchedText(string Text);

    internal readonly record struct AnchorPattern(string Text);

    internal readonly record struct NearbyPattern(string Text);
}
