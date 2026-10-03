using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Benchmarks.Fixtures;

// A distinct lowercase name for every index - a, b, ..., z, aa, ab, ... - for the problems whose
// names and words must be lowercase English letters only, where a numeric suffix such as "loc17"
// would hand the solution digits LeetCode never sends. A name is the bijective base-26 numeral of
// index + 1, so no two indices share one and the shortest names come first: every index below
// 702 is named in at most two letters, every index below 18,278 in at most three.
internal static class LetterNames
{
    private static readonly string Letters = StandardAlphabets.LowercaseLatin.Characters;

    public static string Of(int index)
    {
        var letters = new List<char>();

        for (var numeral = index + 1; numeral > 0; numeral = (numeral - 1) / Letters.Length)
        {
            letters.Add(Letters[(numeral - 1) % Letters.Length]);
        }

        letters.Reverse();

        return new string([.. letters]);
    }
}
