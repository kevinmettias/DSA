using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StringTransformation;

// LeetCode 2851. String Transformation: one operation right-rotates s by an
// arbitrary nonzero amount mod n, so k operations trace a walk of length k on the
// complete graph K_n over s's n rotation positions (from any position, all n-1
// other positions are reachable in exactly one way, one per rotation amount).
// Counting length-k walks between two nodes of K_n has a closed form -
// g(k) = ((n-1)^k - (-1)^k) / n (mod 1e9+7), derived from f(k)-g(k) = (-1)^k and
// f(k) + (n-1)*g(k) = (n-1)^k - elementary modular arithmetic (fast pow + Fermat
// inverse), not itself a repo primitive; k can reach 1e15 so it has to be O(log k),
// ruling out a plain iterative DP over k.
//
// What the closed form needs from string matching is one count: how many of s's n
// rotations equal the target pattern (t when s != t; s itself, i.e. s's period
// count, when s == t). Both strategies below compute that count via a left-rotation
// window search over `s + s[0..n-1]` (every length-n window is exactly one
// left-rotation of s, and by K_n's node-transitive symmetry the walk-count answer
// only depends on the match COUNT, not which specific indexing convention finds
// it) - brute force compares each window character-by-character, the primitive
// version calls this repo's own ZFunction.FindAll, a cross-referential Z-search for
// `pattern` inside `text` with no concatenation/sentinel needed.
public sealed class StringTransformationTests
{
    private const long Mod = 1_000_000_007;

    public static TheoryData<string, string, long, int> Examples =>
        new()
        {
            { "abcd", "cdab", 2, 2 },
            { "ababab", "ababab", 1, 2 },
            { "aab", "abc", 5, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByBruteForceRotationCompare_LeetCodeExamples_ReturnsExpectedCount(
        string s, string t, long k, int expected)
        => Assert.Equal(expected, NumberOfWaysByBruteForceRotationCompare(s, t, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByZFunction_LeetCodeExamples_ReturnsExpectedCount(
        string s, string t, long k, int expected)
        => Assert.Equal(expected, NumberOfWaysByZFunction(s, t, k));

    private static int NumberOfWaysByBruteForceRotationCompare(string s, string t, long k)
    {
        var sameString = s == t;
        var relevantCount = CountRotationMatchesByBruteForce(s, sameString ? s : t);
        return Combine(s.Length, relevantCount, sameString, k);
    }

    private static int NumberOfWaysByZFunction(string s, string t, long k)
    {
        var sameString = s == t;
        var relevantCount = CountRotationMatchesByZFunction(s, sameString ? s : t);
        return Combine(s.Length, relevantCount, sameString, k);
    }

    private static int CountRotationMatchesByBruteForce(string s, string pattern)
    {
        var n = s.Length;
        var count = 0;

        for (var start = 0; start < n; start++)
        {
            var matches = true;

            for (var offset = 0; offset < n; offset++)
            {
                if (s[(start + offset) % n] != pattern[offset])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                count++;
            }
        }

        return count;
    }

    private static int CountRotationMatchesByZFunction(string s, string pattern)
    {
        var text = s + s.Substring(0, s.Length - 1);
        return ZFunction.FindAll(text, pattern).Count;
    }

    // relevantCount is R (matches to t) when s != t, or P (s's own period count,
    // always >= 1) when s == t - the two cases the K_n walk-counting recurrence
    // treats differently: t != s can never be reached by staying at the start node,
    // so every match contributes g(k); t == s can also be reached by staying put,
    // so the start node's own count f(k) plus (P-1) other same-valued nodes apply.
    private static int Combine(int n, int relevantCount, bool sameString, long k)
    {
        var sign = k % 2 == 0 ? 1L : Mod - 1;
        var pow = ModPow(n - 1, k, Mod);
        var inverseN = ModPow(n, Mod - 2, Mod);
        var g = ((pow - sign) % Mod + Mod) % Mod * inverseN % Mod;

        if (!sameString)
        {
            return (int)((long)relevantCount * g % Mod);
        }

        var f = (g + sign) % Mod;
        var extra = (long)(relevantCount - 1) * g % Mod;
        return (int)((f + extra) % Mod);
    }

    private static long ModPow(long value, long exponent, long modulus)
    {
        value %= modulus;
        var result = 1L;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % modulus;
            }

            value = value * value % modulus;
            exponent >>= 1;
        }

        return result;
    }
}
