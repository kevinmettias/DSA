using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

// SegmentTree<Element,TOperation>'s ICombineOperation witness for LC 2213: the
// standard "runs" merge. A run only bridges the boundary when the left range's
// rightmost character equals the right range's leftmost one; when it does, the
// left range's trailing run and the right range's leading run join into one
// candidate, and a whole side that is itself a single run extends the parent's
// prefix or suffix past that side entirely. Associative because the merged value
// only ever depends on the two edge characters, the total length, the runs
// touching the outer edges and the best run anywhere inside - none of which
// depends on how the pieces were grouped.
internal readonly struct RunAggregate : ICombineOperation<RunSegment>
{
    public static RunSegment Identity => RunSegment.Empty;

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
