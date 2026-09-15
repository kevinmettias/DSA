using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

// LeetCode 3008. Find Beautiful Indices in the Given Array II: the identical
// question #3006 asks (every index i where s[i..) starts with a and some
// occurrence of b sits within k positions of it), at a scale where an O(n*m)
// occurrence search and an O(|A|*|B|) pairing scan are both too slow.
//
// The brute force arm below still exists as a first-class, tested strategy -
// it is the baseline the Z-function arm has to beat, and it is what a benchmark
// measures the payoff against - but only the Z-function strategy is fit to run at
// #3008's own published bound.
internal static class FindBeautifulIndicesInTheGivenArrayIISolution
{
    // The textbook double loop for both occurrence searches, then an O(|A| * |B|)
    // scan pairing every a-occurrence against every b-occurrence. Correct on any
    // input, but this is the arm #3008's larger constraints exist to rule out.
    public static int[] FindBeautifulIndicesByBruteForce(Haystack s, PrefixPattern a, NearbyPattern b, int k)
    {
        var aIndices = FindOccurrencesNaive(s, new Needle(a.Text));
        var bIndices = FindOccurrencesNaive(s, new Needle(b.Text));
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

    private static List<int> FindOccurrencesNaive(Haystack s, Needle pattern)
    {
        var matches = new List<int>();

        for (var i = 0; i + pattern.Text.Length <= s.Text.Length; i++)
        {
            var isMatch = true;

            for (var j = 0; j < pattern.Text.Length; j++)
            {
                if (s.Text[i + j] != pattern.Text[j])
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

    // This repo's own Z-function search (Algorithms.StringMatching.ZFunction)
    // finds every occurrence in O(s.Length + pattern.Length); pairing then walks
    // BinarySearch.LowerBound over the b-occurrences (already ascending, since
    // FindAll discovers them left to right) instead of scanning every b for every
    // a - the combination #3008's bound requires.
    public static int[] FindBeautifulIndicesByZFunction(Haystack s, PrefixPattern a, NearbyPattern b, int k)
    {
        var aIndices = ZFunction.FindAll(s.Text, a.Text);
        var bIndices = ZFunction.FindAll(s.Text, b.Text);

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

    // The three strings LC 3008 asks about, named for the roles they play here rather
    // than left as adjacent `string` positions a caller could hand over the wrong way
    // round with the compiler none the wiser. The haystack is the string every
    // occurrence is searched in; the prefix pattern is what an index must start with;
    // the nearby pattern is what must occur within k of it. The three are not
    // interchangeable - the result set is "indices of `a` that have a `b` nearby", and
    // swapping a for b answers a different question entirely. A needle is the one
    // pattern either of those two is reduced to when a shared occurrence scan takes it.
    internal readonly record struct Haystack(string Text);

    internal readonly record struct PrefixPattern(string Text);

    internal readonly record struct NearbyPattern(string Text);

    internal readonly record struct Needle(string Text);
}
