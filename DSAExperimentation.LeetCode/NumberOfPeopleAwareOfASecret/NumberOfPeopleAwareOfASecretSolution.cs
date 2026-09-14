using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfPeopleAwareOfASecret;

// LeetCode 2327. Number of People Aware of a Secret: dp[day] is how many people
// FIRST learn the secret on that day. Day i's newcomers are the sum of dp[j] over
// every day j whose people are still actively sharing on day i - from delay days
// after they learned up to forget - 1 days after - so the recurrence is a
// contiguous range sum over a window of roughly forget - delay earlier days. The
// answer sums dp over every day whose people have not forgotten by day n, which
// is the same range-sum shape against the tail of the array.
//
// Both strategies run that identical recurrence; they differ only in how the
// window sum is obtained. SlidingWindowSum re-adds the whole window on every day;
// FenwickRangeSum keeps dp in this repo's own
// FenwickTree<long, SumOperation<long>> so each day costs one O(log n) Add/Query
// pair instead.
internal static class NumberOfPeopleAwareOfASecretSolution
{
    // Day 1 is dp index 0: LeetCode numbers days from 1, the arrays from 0.
    private const int FirstDay = 1;

    // The textbook answer: dp in a plain array, re-deriving each day's window sum
    // from scratch. O(n * windowSize), which is effectively O(n^2) whenever the
    // window is a fixed fraction of n. Deliberately written with nothing but a
    // BCL array - it is the arm the Fenwick composition has to justify itself
    // against.
    public static long PeopleWithSecretBySlidingWindowSum(int n, int delay, int forget)
    {
        var dp = new long[n];
        dp[0] = 1;

        for (var day = FirstDay + 1; day <= n; day++)
        {
            var low = Math.Max(FirstDay, day - forget + 1);
            var high = day - delay;

            if (high < low)
            {
                continue;
            }

            var sum = 0L;

            for (var source = low; source <= high; source++)
            {
                sum += dp[source - 1];
            }

            dp[day - 1] = sum % ModularArithmetic.Modulo;
        }

        var finalLow = Math.Max(FirstDay, n - forget + 1);
        var total = 0L;

        for (var day = finalLow; day <= n; day++)
        {
            total += dp[day - 1];
        }

        return total % ModularArithmetic.Modulo;
    }

    // The same recurrence against FenwickTree<long, SumOperation<long>>, which is
    // already exactly "point add, prefix/range sum": each day is one Query for the
    // sharing window plus one Add for the newcomers, and the final tail sum is a
    // single Query - O(n log n) overall.
    public static long PeopleWithSecretByFenwickRangeSum(int n, int delay, int forget)
    {
        var dp = new FenwickTree<long, SumOperation<long>>(n);
        dp.Add(0, 1);

        for (var day = FirstDay + 1; day <= n; day++)
        {
            var low = Math.Max(FirstDay, day - forget + 1);
            var high = day - delay;

            if (high >= low)
            {
                var stillSharing = dp.Query(low - 1, high - 1) % ModularArithmetic.Modulo;
                dp.Add(day - 1, stillSharing);
            }
        }

        var finalLow = Math.Max(FirstDay, n - forget + 1);

        return dp.Query(finalLow - 1, n - 1) % ModularArithmetic.Modulo;
    }
}
