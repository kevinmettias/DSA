using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountArrayPairsDivisibleByK;

// LeetCode 2183. Count Array Pairs Divisible by K: count the index pairs i < j whose
// product nums[i] * nums[j] is divisible by the given divisor.
//
// Whether a product is divisible by `divisor` depends only on g_i = Gcd(nums[i], divisor)
// and g_j = Gcd(nums[j], divisor), never on the values themselves, so the two strategies
// differ in whether they ask the question of every ELEMENT pair or of every GROUP pair:
// the distinct gcds are all divisors of `divisor`, so there are at most d of them however
// long the array is, and d is tiny beside n.
internal static class CountArrayPairsDivisibleByKSolution
{
    // n choose 2 = n * (n - 1) / 2 - the count of unordered pairs inside one group.
    private const int DistinctPairDivisor = 2;

    // The textbook answer: test every pair. Deliberately written with nothing but the
    // BCL - it is the arm the grouped composition below has to justify itself against.
    public static long CountPairsByBruteForce(int[] nums, int divisor)
    {
        long pairs = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if ((long)nums[i] * nums[j] % divisor == 0)
                {
                    pairs++;
                }
            }
        }

        return pairs;
    }

    // This repo's own HashMap<TKey, TValue> counting occurrences per computed key - the
    // same idiom TwoSum uses, keyed by a derived gcd instead of a raw value. One pass
    // buckets every index under Gcd(nums[i], divisor), then only the divisor-bounded
    // distinct keys are paired off, so the elementwise O(n^2) scan collapses to O(n + d^2).
    public static long CountPairsByGcdGroups(int[] nums, int divisor)
    {
        var groupCounts = new HashMap<int, int>();

        foreach (var num in nums)
        {
            var group = Gcd(num, divisor);
            groupCounts.TryGetValue(group, out var existing);
            groupCounts.Set(group, existing + 1);
        }

        var groups = new GcdGroups(groupCounts.Keys.ToList(), groupCounts, divisor);
        long pairs = 0;

        for (var groupIndex = 0; groupIndex < groups.Keys.Count; groupIndex++)
        {
            pairs += CountPairsForGroup(groups, groupIndex);
        }

        return pairs;
    }

    // Pairs the group at `groupIndex` with itself and with every later group, which is
    // what keeps each unordered pair counted exactly once.
    private static long CountPairsForGroup(GcdGroups groups, int groupIndex)
    {
        long pairs = 0;

        for (var j = groupIndex; j < groups.Keys.Count; j++)
        {
            if ((long)groups.Keys[groupIndex] * groups.Keys[j] % groups.Divisor != 0)
            {
                continue;
            }

            groups.Counts.TryGetValue(groups.Keys[groupIndex], out var countI);
            groups.Counts.TryGetValue(groups.Keys[j], out var countJ);

            pairs += groupIndex == j
                ? CountPairsWithinOneGroup(countI)
                : CountPairsBetweenGroups(countI, countJ);
        }

        return pairs;
    }

    // The unordered pairs a group of `count` equal-divisor values forms with itself.
    private static long CountPairsWithinOneGroup(int count) =>
        (long)count * (count - 1) / DistinctPairDivisor;

    // The pairs between two distinct groups, every one of which is divisible by the
    // shared divisor.
    private static long CountPairsBetweenGroups(int leftCount, int rightCount) =>
        (long)leftCount * rightCount;

    // The buckets plus the divisor they were computed against, carried together so the
    // pairing walk takes one argument rather than three interchangeable ones.
    private readonly record struct GcdGroups(List<int> Keys, HashMap<int, int> Counts, int Divisor);

    private static int Gcd(int firstOperand, int secondOperand) =>
        secondOperand == 0 ? firstOperand : Gcd(secondOperand, firstOperand % secondOperand);
}
