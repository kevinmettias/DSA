using NeighbourPrefixTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<
    int, DSAExperimentation.Domain.Mathematics.Algebra.Minimum<int>>;

namespace DSAExperimentation.DataStructures.SuffixArray;

// The longest common prefix of any two suffixes, read off a SuffixArray: O(n) to prepare, then
// O(log n) per query. Two suffixes share exactly the shortest of the neighbour prefixes sorted
// between them, so one range minimum over the array's LongestCommonPrefixArray answers a query -
// this repo's SegmentTree<int, Minimum<int>>, as PreOrderLowestCommonAncestor reads its ancestors.
//
// A composition beside the array rather than a part of it, so a caller that never asks pays
// nothing: the tree is built here, never by SuffixArray's constructor. It is the one structure that
// answers this query naturally, so it has no contract of its own (§5 step 3). A suffix tree answers
// it as the depth of two leaves' lowest common ancestor, which takes the same range minimum over
// its leaves in order - the suffix array SuffixTree.ToSuffixArray reads off it. A suffix
// automaton's link tree answers the mirror question, the longest common suffix of two prefixes, not
// this one.
//
// A segment tree rather than a sparse table: O(n) preparation and memory against O(n log n), for a
// log factor per query, and no new structure in the repo. A caller with many more queries than
// characters is the one a sparse table would serve.
/// <summary>
/// Answers the longest common prefix of any two suffixes of a text in O(log n), after O(n)
/// preparation.
/// </summary>
public sealed class LongestCommonPrefixQueries
{
    private const string PositionOutOfBoundsMessage =
        "A suffix starts at a position inside the text.";

    private readonly SuffixArray _suffixArray;
    private readonly NeighbourPrefixTree _neighbourPrefixes;

    public LongestCommonPrefixQueries(SuffixArray suffixArray)
    {
        _suffixArray = suffixArray;
        _neighbourPrefixes = new NeighbourPrefixTree(suffixArray.LongestCommonPrefixArray);
    }

    /// <summary>Measures how long a prefix two suffixes of the text share.</summary>
    /// <param name="firstSuffix">The position in the text where one suffix starts.</param>
    /// <param name="secondSuffix">
    /// The position where the other starts, which may be the first's own.
    /// </param>
    /// <returns>
    /// The length of their longest common prefix, as the array's comparer compares characters; a
    /// suffix shares all of itself with itself.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A position is outside [0, text length).
    /// </exception>
    public int LongestCommonPrefix(int firstSuffix, int secondSuffix)
    {
        var rank = _suffixArray.Rank;
        RangeBounds.ValidateIndex(firstSuffix, rank.Length, PositionOutOfBoundsMessage);
        RangeBounds.ValidateIndex(secondSuffix, rank.Length, PositionOutOfBoundsMessage);

        if (firstSuffix == secondSuffix)
        {
            return rank.Length - firstSuffix;
        }

        return ShortestNeighbourPrefixBetween(rank[firstSuffix], rank[secondSuffix]);
    }

    // The neighbour prefixes between two ranks are those of the pairs from the earlier rank up to
    // the pair that ends at the later one.
    private int ShortestNeighbourPrefixBetween(int firstRank, int secondRank)
    {
        var earlier = Math.Min(firstRank, secondRank);
        var later = Math.Max(firstRank, secondRank);
        return _neighbourPrefixes.Query(earlier, later - 1);
    }
}
