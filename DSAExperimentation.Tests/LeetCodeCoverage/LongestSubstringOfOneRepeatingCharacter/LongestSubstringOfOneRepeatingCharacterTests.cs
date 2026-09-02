using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringOfOneRepeatingCharacter;

// LeetCode 2213. Longest Substring of One Repeating Character: this repo's own
// SegmentTree<Element,TOperation> (the point-update/range-query family
// ARCHITECTURE.md's SegmentTree worked example describes), keyed by index and
// carrying a per-node RunSegment (leftmost/rightmost character, the segment's own
// prefix/suffix run length, and the best run seen anywhere inside it). RunAggregate
// below composes SegmentTree's own ICombineOperation the same way
// NumberOfLongestIncreasingSubsequenceTests' LisAggregate does - the standard
// "runs" merge: a boundary run only bridges two children when the left child's
// rightmost character equals the right child's leftmost one. Each query is answered
// by Update (O(log n)) followed by Query(0, s.Length - 1), which - because the
// queried range exactly covers the tree's own root range - resolves in O(1) by
// reading the root node directly, never descending.
public sealed class LongestSubstringOfOneRepeatingCharacterTests
{
    [Fact]
    public void LongestRepeating_LeetCodeExample_ReturnsExpectedLengthsAfterEachQuery()
    {
        const string S = "babacc";
        const string QueryCharacters = "bcb";
        int[] queryIndices = [1, 3, 3];

        var lengths = LongestRepeating(S, QueryCharacters, queryIndices);

        Assert.Equal([3, 3, 4], lengths);
    }

    [Fact]
    public void LongestRepeating_SingleCharacterString_TracksThatOneCharacter()
    {
        const string S = "a";
        const string QueryCharacters = "b";
        int[] queryIndices = [0];

        var lengths = LongestRepeating(S, QueryCharacters, queryIndices);

        Assert.Equal([1], lengths);
    }

    [Fact]
    public void LongestRepeating_UpdateBreaksAnExistingRun_ShrinksTheMax()
    {
        const string S = "aaaa";
        const string QueryCharacters = "b";
        int[] queryIndices = [1];

        var lengths = LongestRepeating(S, QueryCharacters, queryIndices);

        // "abaa" (indices2-3 -> "aa") still beats the single leftover "a" at index 0.
        Assert.Equal([2], lengths);
    }

    private static int[] LongestRepeating(string s, string queryCharacters, int[] queryIndices)
    {
        var tree = new SegmentTree<RunSegment, RunAggregate>(BuildLeaves(s));
        var lengths = new int[queryCharacters.Length];

        for (var i = 0; i < queryCharacters.Length; i++)
        {
            tree.Update(queryIndices[i], MakeLeaf(queryCharacters[i]));
            lengths[i] = tree.Query(0, s.Length - 1).MaxLen;
        }

        return lengths;
    }

    private static RunSegment[] BuildLeaves(string s)
    {
        var leaves = new RunSegment[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            leaves[i] = MakeLeaf(s[i]);
        }

        return leaves;
    }

    private static RunSegment MakeLeaf(char c) => new(c, c, Len: 1, PrefixLen: 1, SuffixLen: 1, MaxLen: 1);

    // A leaf carries Len=1 and a real character on both edges; Identity (Len=0) is
    // the "empty range" neutral element MaxOperation<Element>.Identity already
    // precedents - never actually read back here, since every query below spans
    // exactly [0, s.Length - 1], which SegmentTree.Query resolves via its
    // full-cover branch without ever combining against Identity, but Combine still
    // handles it correctly for any other range a future caller might query.
    private readonly record struct RunSegment(char Left, char Right, int Len, int PrefixLen, int SuffixLen, int MaxLen);

    private readonly struct RunAggregate : ICombineOperation<RunSegment>
    {
        public static RunSegment Identity => new('\0', '\0', Len: 0, PrefixLen: 0, SuffixLen: 0, MaxLen: 0);

        public static RunSegment Combine(RunSegment left, RunSegment right)
        {
            if (left.Len == 0)
            {
                return right;
            }

            if (right.Len == 0)
            {
                return left;
            }

            var bridges = left.Right == right.Left;
            var prefixLen = bridges && left.PrefixLen == left.Len ? left.Len + right.PrefixLen : left.PrefixLen;
            var suffixLen = bridges && right.SuffixLen == right.Len ? right.Len + left.SuffixLen : right.SuffixLen;
            var maxLen = Math.Max(left.MaxLen, right.MaxLen);

            if (bridges)
            {
                maxLen = Math.Max(maxLen, left.SuffixLen + right.PrefixLen);
            }

            return new RunSegment(left.Left, right.Right, left.Len + right.Len, prefixLen, suffixLen, maxLen);
        }
    }
}
