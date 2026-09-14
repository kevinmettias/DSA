using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountArrayPairsDivisibleByK;

// LeetCode 2183. Count Array Pairs Divisible by K: count the index pairs i < j whose
// product nums[i] * nums[j] is divisible by k.
//
// Whether a product is divisible by k depends only on g_i = Gcd(nums[i], k) and
// g_j = Gcd(nums[j], k), never on the values themselves, so the two strategies differ
// in whether they ask the question of every ELEMENT pair or of every GROUP pair: the
// distinct gcds are all divisors of k, so there are at most d of them however long the
// array is, and d is tiny beside n.
internal static class CountArrayPairsDivisibleByKSolution
{
    // n choose 2 = n * (n - 1) / 2 - the count of unordered pairs inside one group.
    private const int DistinctPairDivisor = 2;

    // The textbook answer: test every pair. Deliberately written with nothing but the
    // BCL - it is the arm the grouped composition below has to justify itself against.
    public static long CountPairsByBruteForce(int[] nums, int k)
    {
        long pairs = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if ((long)nums[i] * nums[j] % k == 0)
                {
                    pairs++;
                }
            }
        }

        return pairs;
    }

    // This repo's own HashMap<TKey, TValue> counting occurrences per computed key - the
    // same idiom TwoSum uses, keyed by a derived gcd instead of a raw value. One pass
    // buckets every index under Gcd(nums[i], k), then only the divisor-bounded distinct
    // keys are paired off, so the elementwise O(n^2) scan collapses to O(n + d^2).
    public static long CountPairsByGcdGroups(int[] nums, int k)
    {
        var groupCounts = new HashMap<int, int>();

        foreach (var num in nums)
        {
            var group = Gcd(num, k);
            groupCounts.TryGetValue(group, out var existing);
            groupCounts.Set(group, existing + 1);
        }

        var groups = new GcdGroups(groupCounts.Keys.ToList(), groupCounts, k);
        long pairs = 0;

        for (var i = 0; i < groups.Keys.Count; i++)
        {
            pairs += CountPairsForGroup(groups, i);
        }

        return pairs;
    }

    // Pairs group i with itself and with every later group, which is what keeps each
    // unordered pair counted exactly once.
    private static long CountPairsForGroup(GcdGroups groups, int i)
    {
        long pairs = 0;

        for (var j = i; j < groups.Keys.Count; j++)
        {
            if ((long)groups.Keys[i] * groups.Keys[j] % groups.Divisor != 0)
            {
                continue;
            }

            groups.Counts.TryGetValue(groups.Keys[i], out var countI);
            groups.Counts.TryGetValue(groups.Keys[j], out var countJ);

            pairs += i == j ? (long)countI * (countI - 1) / DistinctPairDivisor : (long)countI * countJ;
        }

        return pairs;
    }

    // The buckets plus the divisor they were computed against, carried together so the
    // pairing walk takes one argument rather than three interchangeable ones.
    private readonly record struct GcdGroups(List<int> Keys, HashMap<int, int> Counts, int Divisor);

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
