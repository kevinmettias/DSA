using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FirstBadVersion;

// LeetCode 278. First Bad Version: LeetCode's isBadVersion oracle flips from good to
// bad at some unknown version and never flips back; find the first bad one. This repo
// models the oracle as a `firstBad` threshold rather than a live judge API, but both
// strategies still only ever ask it "is this one version bad?", exactly as the real
// API allows.
internal static class FirstBadVersionSolution
{
    // The textbook answer: call the oracle once per version, in order, until it turns
    // bad - O(n) worst case. Deliberately BCL-only; the oracle check is the only thing
    // either strategy is allowed to know about the input's shape.
    public static int FirstBadVersionByLinearScan(int n, int firstBad)
    {
        for (var version = 1; version <= n; version++)
        {
            if (IsBadVersion(version, firstBad))
            {
                return version;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static bool IsBadVersion(int version, int firstBad) => version >= firstBad;

    // This repo's own BinarySearch.LowerBound over a monotone virtual sequence - the
    // oracle is computed on demand via Get, so the O(n) version history is never
    // materialized, and LowerBound finds the flip point in O(log n) probes.
    public static int FirstBadVersionByLowerBound(int n, int firstBad)
    {
        var sequence = new IsBadVersionSequence(firstBad, n);

        return BinarySearch.LowerBound<int, IsBadVersionSequence>(sequence, 1) + 1;
    }

    // index is the 0-based version-1: every version from firstBad onward is bad, and
    // the sequence never actually allocates the n versions it represents. A witness
    // for this problem alone - the flip-point formula is First Bad Version's own
    // content, not a general "virtual monotone sequence" shape.
    private readonly struct IsBadVersionSequence(int firstBad, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => IsBadVersion(index) ? 1 : 0;

        // index is the 0-based version-1, so version index + 1 is bad from firstBad on.
        private bool IsBadVersion(int index) => index + 1 >= firstBad;
    }
}
