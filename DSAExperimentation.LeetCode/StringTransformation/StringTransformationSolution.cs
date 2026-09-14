using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.StringTransformation;

// LeetCode 2851. String Transformation: one operation removes a nonempty proper
// suffix of s and puts it back on the front, i.e. right-rotates s by an arbitrary
// nonzero amount mod n. So k operations trace a walk of length k on the complete
// graph K_n over s's n rotation positions - from any position all n-1 other
// positions are reachable in exactly one way, one per rotation amount - and the
// answer is the number of such walks that land on a position spelling t.
//
// Counting length-k walks between two nodes of K_n has a closed form:
// g(k) = ((n-1)^k - (-1)^k) / n and f(k) = g(k) + (-1)^k (mod 1e9+7), from
// f(k) - g(k) = (-1)^k and f(k) + (n-1)*g(k) = (n-1)^k. That is elementary modular
// arithmetic - Domain.Modular's fast power and Fermat inverse - not a repo
// primitive; k reaches 1e15, so it has to be O(log k), which rules out an
// iterative DP over k for BOTH arms. The closed form is therefore shared, and the
// one thing the two strategies actually differ in is the single count it needs:
// how many of s's n rotations spell the target (t when s != t; s itself, i.e. s's
// period count, when s == t).
//
// Both compute that count as a window search over s + s[0..n-2], where every
// length-n window is exactly one left-rotation of s. By K_n's node-transitive
// symmetry the walk count depends only on HOW MANY positions spell the target, not
// on which indexing convention enumerates them, so left- vs right-rotation
// indexing is immaterial here.
//
// NumberOfWaysByBruteForceRotationCompare is the textbook arm: compare each window
// character by character, O(n^2). NumberOfWaysByZFunction calls this repo's
// ZFunction.FindAll, a cross-referential Z-search for a pattern inside a text that
// needs no concatenation or sentinel, O(n).
internal static class StringTransformationSolution
{
    // (-1)^k reduced mod 1e9+7 so the closed form stays in nonnegative residues:
    // +1 when k is even, and Modulo - 1 - which IS -1 modulo the prime - when odd.
    private const long EvenExponentSign = 1;
    private const long OddExponentSign = ModularArithmetic.Modulo - EvenExponentSign;

    // The textbook baseline: rebuild every rotation position in place and compare
    // it against the target one character at a time. Deliberately written without
    // this repo's string-matching primitives - it is the O(n^2) arm the Z-function
    // strategy below has to justify itself against. (The closed-form tail it shares
    // with that strategy is not part of the contrast: modular exponentiation by
    // squaring is what you would write inline either way, and 1e9+7 is LeetCode's
    // reporting convention, which is exactly why Domain.Modular owns both.)
    public static int NumberOfWaysByBruteForceRotationCompare(string source, string target, long operations)
    {
        var targetIsSource = source == target;
        var matchCount = CountRotationMatchesByBruteForce(source, targetIsSource ? source : target);

        return targetIsSource
            ? WaysBackToAnEqualRotation(source.Length, matchCount, operations)
            : WaysOntoADifferentRotation(source.Length, matchCount, operations);
    }

    private static int CountRotationMatchesByBruteForce(string source, string pattern)
    {
        var count = 0;

        for (var start = 0; start < source.Length; start++)
        {
            if (IsRotationEqualToPattern(source, start, pattern))
            {
                count++;
            }
        }

        return count;
    }

    // The left-rotation of source beginning at `start`, compared character by
    // character against the pattern without ever materializing the rotated string.
    private static bool IsRotationEqualToPattern(string source, int start, string pattern)
    {
        var length = source.Length;

        for (var offset = 0; offset < length; offset++)
        {
            if (source[(start + offset) % length] != pattern[offset])
            {
                return false;
            }
        }

        return true;
    }

    // Same closed form, but the rotation-match count comes from ZFunction.FindAll:
    // one Z-array over the pattern plus one cross-referential scan of the doubled
    // text, O(n) total instead of O(n^2).
    public static int NumberOfWaysByZFunction(string source, string target, long operations)
    {
        var targetIsSource = source == target;
        var matchCount = CountRotationMatchesByZFunction(source, targetIsSource ? source : target);

        return targetIsSource
            ? WaysBackToAnEqualRotation(source.Length, matchCount, operations)
            : WaysOntoADifferentRotation(source.Length, matchCount, operations);
    }

    // source + source[0..n-2] holds every length-n rotation as a window and no
    // spurious extra one: dropping the last character is what stops the full
    // doubling from reporting the wrap-around window at index n a second time.
    private static int CountRotationMatchesByZFunction(string source, string pattern)
    {
        var text = source + source[..^1];

        return ZFunction.FindAll(text, pattern).Count;
    }

    // t != s can never be reached by a walk that ends where it started, so each of
    // the matchCount positions spelling t contributes g(k).
    private static int WaysOntoADifferentRotation(int length, int matchCount, long operations)
        => (int)((long)matchCount * WalksToADifferentNode(length, operations) % ModularArithmetic.Modulo);

    // t == s can also be reached by returning to the start node, so the start's own
    // f(k) applies once and the other periodCount - 1 positions spelling s each
    // contribute g(k). periodCount is always at least 1 (s matches itself).
    private static int WaysBackToAnEqualRotation(int length, int periodCount, long operations)
    {
        var walksElsewhere = WalksToADifferentNode(length, operations);
        var walksBack = (walksElsewhere + ParitySign(operations)) % ModularArithmetic.Modulo;
        var walksToAnEqualPeer = (long)(periodCount - 1) * walksElsewhere % ModularArithmetic.Modulo;

        return (int)((walksBack + walksToAnEqualPeer) % ModularArithmetic.Modulo);
    }

    // g(k) = ((n-1)^k - (-1)^k) / n: length-k walks on K_n between two distinct
    // nodes. Dividing by n is multiplying by its Fermat inverse mod 1e9+7.
    private static long WalksToADifferentNode(int length, long operations)
    {
        var power = ModularArithmetic.Power(length - 1, operations);
        var difference = (power - ParitySign(operations) + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;

        return difference * ModularArithmetic.Inverse(length) % ModularArithmetic.Modulo;
    }

    // (-1)^k, the alternating term in both closed forms.
    private static long ParitySign(long operations)
    {
        if (long.IsEvenInteger(operations))
        {
            return EvenExponentSign;
        }

        return OddExponentSign;
    }
}
