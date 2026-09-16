namespace DSAExperimentation.LeetCode.MaximizeSubarrayGCDScore;

// LeetCode 3574. Maximize Subarray GCD Score: pick a contiguous subarray and
// double up to maxDoubledElements of its own elements (each element doubled at
// most once) to maximize length * gcd(subarray). Doubling only ever touches the
// power-of-two factor a gcd carries - every element's ODD part is untouched by
// doubling, so gcd(elements)'s odd part can never change, and any element already
// at the subarray's minimum power-of-two exponent can gain at most +1 (one double,
// ever), which caps the achievable gcd at exactly double the original: doable
// only when every element AT that minimum exponent can be doubled, i.e. their
// count doesn't exceed maxDoubledElements.
internal static class MaximizeSubarrayGCDScoreSolution
{
    // The bottleneck scan's running state for the window so far: its gcd, the
    // smallest power-of-two exponent any of its elements has, and how many
    // elements share that exponent.
    private readonly record struct WindowScan(int Gcd, int MinExponent, int MinExponentCount);

    // The literal reading of the problem: every subarray, every subset of its
    // own indices (up to k of them) tried as the doubled set, gcd recomputed
    // from scratch each time. Genuinely exponential in subarray length - the
    // arm the bottleneck-scan strategy below has to beat.
    public static long MaxScoreByBruteForce(int[] nums, int maxDoubledElements)
    {
        var best = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            for (var right = left; right < nums.Length; right++)
            {
                var score = BestScoreForSubarrayByTryingEverySubset(nums, left, right, maxDoubledElements);
                best = Math.Max(best, score);
            }
        }

        return best;
    }

    private static long BestScoreForSubarrayByTryingEverySubset(
        int[] nums, int left, int right, int maxDoubledElements)
    {
        var length = right - left + 1;
        var subsetCount = 1 << length;
        var best = 0L;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            if (PopCount(mask) > maxDoubledElements)
            {
                continue;
            }

            var score = ScoreForDoubledSubset(nums, left, length, mask);
            best = Math.Max(best, score);
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

    // The score of doubling exactly the indices `mask` selects - the subarray's
    // length times the gcd the doubled elements leave behind. The caller has
    // already rejected the masks that double more elements than maxDoubledElements
    // allows.
    private static long ScoreForDoubledSubset(int[] nums, int left, int length, int mask)
    {
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

        return (long)length * gcd;
    }

    // One O(n) pass per left endpoint, tracking the running gcd plus the
    // minimum power-of-two exponent seen so far and how many elements share it
    // - the only two facts a subarray's best achievable score depends on, per
    // this class's own doc comment.
    public static long MaxScoreByBottleneckGcdScan(int[] nums, int maxDoubledElements)
    {
        var best = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            best = BestFromLeftEndpoint(nums, left, maxDoubledElements, best);
        }

        return best;
    }

    // One O(n) pass over every window starting at `left`: length * gcd, doubled
    // when the elements sitting at the window's minimum power-of-two exponent
    // number no more than maxDoubledElements, so all of them can be doubled.
    private static long BestFromLeftEndpoint(int[] nums, int left, int maxDoubledElements, long best)
    {
        var scan = new WindowScan(0, int.MaxValue, 0);

        for (var right = left; right < nums.Length; right++)
        {
            scan = AbsorbElement(nums[right], scan);

            var multiplier = scan.MinExponentCount <= maxDoubledElements ? 2 : 1;
            best = Math.Max(best, (long)(right - left + 1) * scan.Gcd * multiplier);
        }

        return best;
    }

    // Folds one more element into the window's running state: its running gcd,
    // and its minimum power-of-two exponent with the count sharing it.
    private static WindowScan AbsorbElement(int value, WindowScan scan)
    {
        var runningGcd = Gcd(scan.Gcd, value);
        var exponent = TrailingZeroCount(value);

        if (exponent < scan.MinExponent)
        {
            return new WindowScan(runningGcd, exponent, 1);
        }

        if (exponent > scan.MinExponent)
        {
            return new WindowScan(runningGcd, scan.MinExponent, scan.MinExponentCount);
        }

        return new WindowScan(runningGcd, scan.MinExponent, scan.MinExponentCount + 1);
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

    private static int Gcd(int firstOperand, int secondOperand) =>
        secondOperand == 0 ? firstOperand : Gcd(secondOperand, firstOperand % secondOperand);
}
