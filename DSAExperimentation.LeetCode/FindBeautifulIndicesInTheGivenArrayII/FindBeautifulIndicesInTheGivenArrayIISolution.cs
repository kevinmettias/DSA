using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

// LeetCode 3008. Find Beautiful Indices in the Given Array II: the identical
// question #3006 asks (every start index where the haystack from there starts with
// the prefix pattern and some occurrence of the nearby pattern sits within
// maxDistance positions of it), at a scale where an O(n*m) occurrence search and an
// O(|A|*|B|) pairing scan are both too slow.
//
// The brute force arm below still exists as a first-class, tested strategy -
// it is the baseline the Z-function arm has to beat, and it is what a benchmark
// measures the payoff against - but only the Z-function strategy is fit to run at
// #3008's own published bound.
internal static class FindBeautifulIndicesInTheGivenArrayIISolution
{
    // The textbook double loop for both occurrence searches, then an O(|A| * |B|)
    // scan pairing every prefix occurrence against every nearby occurrence. Correct
    // on any input, but this is the arm #3008's larger constraints exist to rule out.
    public static int[] FindBeautifulIndicesByBruteForce(
        Haystack haystack, PrefixPattern prefixPattern, NearbyPattern nearbyPattern, int maxDistance)
    {
        var aIndices = FindOccurrencesNaive(haystack, new Needle(prefixPattern.Text));
        var bIndices = FindOccurrencesNaive(haystack, new Needle(nearbyPattern.Text));
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

    private static List<int> FindOccurrencesNaive(Haystack haystack, Needle pattern)
    {
        var matches = new List<int>();

        for (var i = 0; i + pattern.Text.Length <= haystack.Text.Length; i++)
        {
            var isMatch = true;

            for (var j = 0; j < pattern.Text.Length; j++)
            {
                if (haystack.Text[i + j] != pattern.Text[j])
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

    // This repo's own Z-function search (Algorithms.StringMatching.ZFunction)
    // finds every occurrence in O(haystack.Length + pattern.Length); pairing then
    // walks BinarySearch.LowerBound over the nearby occurrences (already ascending,
    // since FindAll discovers them left to right) instead of scanning every nearby
    // occurrence for every prefix occurrence - the combination #3008's bound requires.
    public static int[] FindBeautifulIndicesByZFunction(
        Haystack haystack, PrefixPattern prefixPattern, NearbyPattern nearbyPattern, int maxDistance)
    {
        var aIndices = ZFunction.FindAll(haystack.Text, prefixPattern.Text);
        var bIndices = ZFunction.FindAll(haystack.Text, nearbyPattern.Text);

        return CollectNearbyIndices(aIndices, bIndices, maxDistance);
    }

    // aIndices and bIndices are both already ascending, so LowerBound(index - maxDistance)
    // locates the first nearby occurrence that could possibly be within maxDistance of
    // index in one O(log |B|) probe instead of a linear scan of bIndices per prefix index.
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

    // The three strings LC 3008 asks about, named for the roles they play here rather
    // than left as adjacent `string` positions a caller could hand over the wrong way
    // round with the compiler none the wiser. The haystack is the string every
    // occurrence is searched in; the prefix pattern is what an index must start with;
    // the nearby pattern is what must occur within maxDistance of it. The three are not
    // interchangeable - the result set is "indices of the prefix pattern that have a
    // nearby-pattern occurrence close by", and swapping prefix for nearby answers a
    // different question entirely. A needle is the one pattern either of those two is
    // reduced to when a shared occurrence scan takes it.
    internal readonly record struct Haystack(string Text);

    internal readonly record struct PrefixPattern(string Text);

    internal readonly record struct NearbyPattern(string Text);

    internal readonly record struct Needle(string Text);
}
