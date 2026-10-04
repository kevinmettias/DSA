using DSAExperimentation.Algorithms.Searching;

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
    public static int FirstBadVersionByLinearScan(int versionCount, int firstBad)
    {
        for (var version = 1; version <= versionCount; version++)
        {
            if (IsBadVersion(version, firstBad))
            {
                return version;
            }
        }

        return LeetCodeAnswer.None;
    }

    private static bool IsBadVersion(int version, int firstBad) => version >= firstBad;

    // This repo's own MonotonePredicateSearch over the versions 1..versionCount - the
    // oracle is asked on demand, so the O(n) version history is never materialized,
    // and FirstTrue finds the flip point in O(log n) probes. The range is stated in
    // long because LeetCode allows versionCount = int.MaxValue, and an int range
    // ending there leaves no room for the "none is bad" answer one past it.
    public static int FirstBadVersionByPredicateSearch(int versionCount, int firstBad) =>
        (int)MonotonePredicateSearch.FirstTrue(1L, versionCount, new VersionIsBad(firstBad));

    // IsSatisfiedBy(version) asks the same oracle the linear scan asks - good before firstBad,
    // bad from there on and never good again, the monotonicity FirstTrue assumes but
    // never checks. Every version asked about lies in 1..versionCount, so narrowing it
    // back to int is exact. A rule for this problem alone: the oracle is First Bad
    // Version's own content.
    private readonly struct VersionIsBad(int firstBad) : IMonotonePredicate<long>
    {
        public bool IsSatisfiedBy(long version) => IsBadVersion((int)version, firstBad);
    }
}
