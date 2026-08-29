namespace DSAExperimentation.DataStructures;

// Shared index/range precondition checks for the SegmentTree/LazySegmentTree/FenwickTree/
// RangeFenwickTree family - not a shared Representation type, just the one bounds check
// each would otherwise redeclare identically (the same class of cross-file duplication a
// repo-wide structural-duplication check already flagged once, resolved for Split's
// arithmetic by SegmentRange.cs). Every call site across all four types already threw
// nameof(index)/nameof(left) for these exact checks, so the parameter name isn't
// caller-specific and doesn't need to be threaded through - only the message text, which
// stays per-type so a thrown exception still names which tree it came from.
internal static class RangeBounds
{
    public static void ValidateIndex(int index, int size, string message)
    {
        if (index < 0 || index >= size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), message);
        }
    }

    public static void ValidateRange(int left, int right, int size, string message)
    {
        var isOutOfRange = left < 0 || right >= size || left > right;

        if (isOutOfRange)
        {
            throw new ArgumentOutOfRangeException(nameof(left), message);
        }
    }
}
