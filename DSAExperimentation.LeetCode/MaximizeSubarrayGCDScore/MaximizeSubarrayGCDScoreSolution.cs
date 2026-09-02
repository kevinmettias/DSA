namespace DSAExperimentation.LeetCode.MaximizeSubarrayGCDScore;

// LeetCode 3574. Maximize Subarray GCD Score: pick a contiguous subarray and
// double up to k of its own elements (each element doubled at most once) to
// maximize length * gcd(subarray). Doubling only ever touches the power-of-two
// factor a gcd carries - every element's ODD part is untouched by doubling, so
// gcd(elements)'s odd part can never change, and any element already at the
// subarray's minimum power-of-two exponent can gain at most +1 (one double,
// ever), which caps the achievable gcd at exactly double the original: doable
// only when every element AT that minimum exponent can be doubled, i.e. their
// count doesn't exceed k.
internal static class MaximizeSubarrayGCDScoreSolution
{
    // The literal reading of the problem: every subarray, every subset of its
    // own indices (up to k of them) tried as the doubled set, gcd recomputed
    // from scratch each time. Genuinely exponential in subarray length - the
    // arm the bottleneck-scan strategy below has to beat.
    public static long MaxScoreByBruteForce(int[] nums, int k)
    {
        var best = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            for (var right = left; right < nums.Length; right++)
            {
                best = Math.Max(best, BestScoreForSubarrayByTryingEverySubset(nums, left, right, k));
            }
        }

        return best;
    }

    private static long BestScoreForSubarrayByTryingEverySubset(int[] nums, int left, int right, int k)
    {
        var length = right - left + 1;
        var subsetCount = 1 << length;
        var best = 0L;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            if (PopCount(mask) > k)
            {
                continue;
            }

            var gcd = 0;

            for (var offset = 0; offset < length; offset++)
            {
                var value = nums[left + offset];

                if ((mask & (1 << offset)) != 0)
                {
                    value *= 2;
                }

                gcd = Gcd(gcd, value);
            }

            best = Math.Max(best, (long)length * gcd);
        }

        return best;
    }

    private static int PopCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            count += mask & 1;
            mask >>= 1;
        }

        return count;
    }

    // One O(n) pass per left endpoint, tracking the running gcd plus the
    // minimum power-of-two exponent seen so far and how many elements share it
    // - the only two facts a subarray's best achievable score depends on, per
    // this class's own doc comment.
    public static long MaxScoreByBottleneckGcdScan(int[] nums, int k)
    {
        var best = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var runningGcd = 0;
            var minExponent = int.MaxValue;
            var minExponentCount = 0;

            for (var right = left; right < nums.Length; right++)
            {
                runningGcd = Gcd(runningGcd, nums[right]);
                var exponent = TrailingZeroCount(nums[right]);

                if (exponent < minExponent)
                {
                    minExponent = exponent;
                    minExponentCount = 1;
                }
                else if (exponent == minExponent)
                {
                    minExponentCount++;
                }

                var length = right - left + 1;
                var multiplier = minExponentCount <= k ? 2 : 1;

                best = Math.Max(best, (long)length * runningGcd * multiplier);
            }
        }

        return best;
    }

    private static int TrailingZeroCount(int value)
    {
        var count = 0;

        while ((value & 1) == 0)
        {
            value >>= 1;
            count++;
        }

        return count;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
