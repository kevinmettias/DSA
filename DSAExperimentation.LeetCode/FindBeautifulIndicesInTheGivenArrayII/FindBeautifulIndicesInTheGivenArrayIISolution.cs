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
    public static int[] FindBeautifulIndicesByBruteForce(string s, string a, string b, int k)
    {
        var aIndices = FindOccurrencesNaive(s, a);
        var bIndices = FindOccurrencesNaive(s, b);
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

    private static List<int> FindOccurrencesNaive(string s, string pattern)
    {
        var matches = new List<int>();

        for (var i = 0; i + pattern.Length <= s.Length; i++)
        {
            var isMatch = true;

            for (var j = 0; j < pattern.Length; j++)
            {
                if (s[i + j] != pattern[j])
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
    public static int[] FindBeautifulIndicesByZFunction(string s, string a, string b, int k)
    {
        var aIndices = ZFunction.FindAll(s, a);
        var bIndices = ZFunction.FindAll(s, b);

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
}
