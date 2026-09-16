using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.FindSubstringWithGivenHashValue;

// LeetCode 2156. Find Substring With Given Hash Value: report the FIRST length-k
// substring of s whose hash - sum(val(c_j) * power^j) mod modulo, with val(c) the
// letter's alphabet index plus one and the WINDOW'S OWN FIRST character on
// power^0 - equals hashValue.
//
// LeetCode's (power, modulo) pair is exactly one polynomial-hash tuning lane, so
// it is taken as this repo's own RollingHashLane rather than as two bare longs a
// caller could transpose - the same grouping RollingHashLane's own doc comment
// argues for, and the type the pre-migration test had already re-invented locally
// as a private `RollingHashLaneConfig(Power, Modulo)` record.
//
// The two strategies differ only in how a window's hash is obtained: recomputed
// from its characters every time, or read off a precomputed prefix table in
// O(1).
//
// LeetCode guarantees an answer exists, so the "no window matches" branch never
// fires on real input - but it is reachable (a benchmark deliberately asks for an
// unreachable hash value so neither strategy can exit early), which is why both
// strategies use this repo's ordinary Try shape instead of throwing, matching
// TwoSumSolution.TryFindIndicesBy* under exactly the same benchmark trick.
internal static class FindSubstringWithGivenHashValueSolution
{
    // LC 2156's own character value, injected into RollingHash through the same
    // IEqualityComparer<char> hook RollingHashTests' case-insensitive-comparer
    // test already exercises.
    private static readonly IEqualityComparer<char> LetterValues =
        EqualityComparer<char>.Create((left, right) => left == right, LetterValue);

    // The textbook answer: walk each window's characters and rebuild its modular
    // hash from scratch, O(n * k) overall. Deliberately BCL-only - a bare long
    // accumulator and a running power term - since it is the arm the precomputed
    // strategy below has to justify itself against. Only the (power, modulo) pair
    // it is handed is a repo type.
    // The thing being looked for is one hand: a window length and the hash that
    // window must carry. Neither means anything without the other, so they are
    // passed as the one query they describe.
    public static bool TryFindSubstringByWindowRehash(
        string text, RollingHashLane lane, (int WindowLength, long HashValue) target, out string substring)
    {
        for (var start = 0; start <= text.Length - target.WindowLength; start++)
        {
            if (WindowHash(text, start, target.WindowLength, lane) == target.HashValue)
            {
                substring = text.Substring(start, target.WindowLength);
                return true;
            }
        }

        substring = string.Empty;
        return false;
    }

    private static long WindowHash(string text, int start, int windowLength, RollingHashLane lane)
    {
        long hash = 0;
        long powerTerm = 1;

        for (var offset = 0; offset < windowLength; offset++)
        {
            hash = (hash + (LetterValue(text[start + offset]) * powerTerm)) % lane.Modulus;
            powerTerm = powerTerm * lane.Base % lane.Modulus;
        }

        return hash;
    }

    // This repo's own RollingHash, built once in O(n) and then queried in O(1) per
    // window - queried over the REVERSED text, because the two hash formulas
    // assign powers in opposite directions.
    //
    // RollingHash.Hash(start, length) combines a window as
    // sum(val(text[start+j]) * base^(length-1-j)) - the LEFTMOST character of the
    // queried window gets the HIGHEST power (confirmed by RollingHashTests' own
    // hand-computed "ab"/base-10/mod-97 example: Hash(0,2) = val('a')*10 +
    // val('b')*1). LC 2156 wants the opposite: val(s[0]) on power^0, val(s[k-1])
    // on power^(k-1). A window of the REVERSED text produces exactly that - its
    // leftmost character is the original window's LAST character, so it lands on
    // the highest power - so window [i, i+k) of an n-character text maps to
    // [n-i-k, n-i) of the reversed text.
    //
    // The same lane is passed as both of RollingHash's lanes: it always double
    // hashes for collision resistance (RollingHash.cs's own doc comment), but this
    // problem's hashValue is an exact modular value to match under LeetCode's own
    // single modulus, not a probabilistic screen, so only the First component is
    // ever compared.
    public static bool TryFindSubstringByRollingHash(
        string text, RollingHashLane lane, (int WindowLength, long HashValue) target, out string substring)
    {
        var reversedHash = BuildReversedWindowHash(text, lane);

        return TryFindSubstringByRollingHash(reversedHash, text, target, out substring);
    }

    // The hoisted overload: takes the prefix table already built, so a benchmark's
    // [GlobalSetup] can charge the O(n) construction to setup rather than to the
    // measured window sweep. reversedHash must be the table BuildReversedWindowHash
    // returns for this same text.
    public static bool TryFindSubstringByRollingHash(
        RollingHash reversedHash, string text, (int WindowLength, long HashValue) target, out string substring)
    {
        var n = text.Length;

        for (var start = 0; start <= n - target.WindowLength; start++)
        {
            if (reversedHash.Hash(n - start - target.WindowLength, target.WindowLength).First
                == target.HashValue)
            {
                substring = text.Substring(start, target.WindowLength);
                return true;
            }
        }

        substring = string.Empty;
        return false;
    }

    // The prepared input the composed strategy's hoisted overload takes: this
    // text's prefix hash table, over the reversed characters and under LeetCode's
    // own lane.
    public static RollingHash BuildReversedWindowHash(string text, RollingHashLane lane)
    {
        var reversed = text.ToCharArray();
        Array.Reverse(reversed);

        return new RollingHash(reversed, LetterValues, lane, lane);
    }

    private static int LetterValue(char letter) => letter - 'a' + 1;
}
