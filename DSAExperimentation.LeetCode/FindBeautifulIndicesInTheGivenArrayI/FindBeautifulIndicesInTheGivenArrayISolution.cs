using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

namespace DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

// LeetCode 3006. Find Beautiful Indices in the Given Array I: every start index where
// the searched text from there starts with the anchor pattern and some occurrence of
// the nearby pattern sits within maxDistance positions of it.
//
// Both strategies reduce to the same two steps - find every occurrence of the anchor
// pattern and of the nearby pattern in the searched text, then keep the anchor
// occurrences that have a nearby occurrence within maxDistance - and differ only in
// how the occurrences are found. #3008 asks the identical question at a scale this
// file's brute force cannot reach, so its class owns the brute force and the pairing
// step (ARCHITECTURE 17.3) and this class calls in for them; the prefix-function
// search stays this class's own, as #3008's is the Z-function.
internal static class FindBeautifulIndicesInTheGivenArrayISolution
{
    // The textbook double loop for both occurrence searches, then an O(|A| * |B|)
    // scan pairing every anchor occurrence against every nearby occurrence - the arm
    // the composed strategy below has to beat.
    public static int[] FindBeautifulIndicesByBruteForce(
        SearchedText searchedText, AnchorPattern anchorPattern, NearbyPattern nearbyPattern, int maxDistance) =>
        FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByBruteForce(
            new(searchedText.Text), new(anchorPattern.Text), new(nearbyPattern.Text), maxDistance);

    // This repo's own KMP search (Algorithms.StringMatching.PrefixFunctionSearch)
    // finds every occurrence in O(searchedText.Length + pattern.Length); the pairing
    // then probes the ascending nearby occurrences by binary search instead of
    // scanning them all for every anchor occurrence.
    public static int[] FindBeautifulIndicesByPrefixFunctionSearch(
        SearchedText searchedText, AnchorPattern anchorPattern, NearbyPattern nearbyPattern, int maxDistance)
    {
        var aIndices = PrefixFunctionSearch.FindAll(searchedText.Text, anchorPattern.Text);
        var bIndices = PrefixFunctionSearch.FindAll(searchedText.Text, nearbyPattern.Text);

        return FindBeautifulIndicesInTheGivenArrayIISolution.CollectNearbyIndices(aIndices, bIndices, maxDistance);
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
