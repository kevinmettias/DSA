namespace DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

// LeetCode 2654. Minimum Number of Operations to Make All Array Elements Equal
// to 1: one operation replaces an element with the gcd of itself and a
// neighbour, and the answer is the fewest operations that leave every element
// at 1.
//
// If a 1 is already present it can be walked along the array, so every other
// element costs exactly one operation - n - ones, and nothing else to decide.
// Otherwise Bezout's identity says the shortest contiguous window whose own gcd
// is 1 can be collapsed to a single 1 in (windowLength - 1) operations, and
// spreading that 1 over the rest costs (n - 1) more. When no window's gcd ever
// reaches 1 the whole array's gcd is not 1 either, and no sequence of operations
// can produce one: LeetCode reports -1.
//
// The window scan is identical either way, so the only strategy choice the
// problem leaves is how each pairwise gcd step is computed: repeated subtraction
// (the textbook first gcd algorithm, O(max/min) per pair) against the
// modulo-based Euclidean algorithm (O(log min) per pair). That is the same
// contrast CheckIfItIsAGoodArraySolution and FindGreatestCommonDivisorOfArray-
// Solution draw for their own gcd folds, here paid once per window rather than
// once for the whole array - which is what makes it visible. As those two record,
// a reduction over a running integer has no repo container or algorithm
// primitive to compose over, so the gcd helpers stay inline here too.
internal static class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution
{
    // The textbook answer: the subtraction form of Euclid's algorithm driving the
    // window scan. Deliberately written without this repo's primitives - it is
    // the arm the Euclidean scan below has to justify itself against.
    public static int MinOperationsBySubtractionGcd(int[] nums)
    {
        var ones = CountOnes(nums);

        return ones > 0 ? nums.Length - ones : SpreadCost(nums.Length, ShortestSubtractionGcdWindow(nums));
    }

    // The modulo form of the same scan: each step replaces the running pair with
    // (b, a % b), so the values shrink geometrically instead of by one multiple
    // of the smaller at a time.
    public static int MinOperationsByEuclideanGcd(int[] nums)
    {
        var ones = CountOnes(nums);

        return ones > 0 ? nums.Length - ones : SpreadCost(nums.Length, ShortestEuclideanGcdWindow(nums));
    }

    // Building one 1 costs (window - 1) operations; spreading it over the array
    // costs (n - 1) more. No window at all means no 1 is reachable, which is
    // LeetCode's -1. The window helpers report that as null rather than sharing
    // this tier's answer sentinel, the same reason HammingSearch returns int?
    // instead of exporting an Unreachable constant (ARCHITECTURE.md 17.9).
    private static int SpreadCost(int length, int? shortestWindow)
        => shortestWindow is null ? LeetCodeAnswer.None : (shortestWindow.Value - 1) + (length - 1);

    private static int CountOnes(int[] nums)
    {
        var ones = 0;

        foreach (var value in nums)
        {
            if (value == 1)
            {
                ones++;
            }
        }

        return ones;
    }

    // Windows of length 1 are deliberately not considered: an element that is
    // already 1 is handled by the ones count, and no other single element has
    // gcd 1 with itself.
    private static int? ShortestSubtractionGcdWindow(int[] nums)
    {
        int? best = null;

        for (var start = 0; start < nums.Length; start++)
        {
            var running = nums[start];

            for (var end = start + 1; end < nums.Length; end++)
            {
                running = SubtractionGcd(running, nums[end]);

                if (running == 1)
                {
                    best = Shorter(best, end - start + 1);
                    break;
                }
            }
        }

        return best;
    }

    private static int? ShortestEuclideanGcdWindow(int[] nums)
    {
        int? best = null;

        for (var start = 0; start < nums.Length; start++)
        {
            var running = nums[start];

            for (var end = start + 1; end < nums.Length; end++)
            {
                running = EuclideanGcd(running, nums[end]);

                if (running == 1)
                {
                    best = Shorter(best, end - start + 1);
                    break;
                }
            }
        }

        return best;
    }

    private static int Shorter(int? best, int length)
        => best is null ? length : Math.Min(best.Value, length);

    // Repeatedly subtract the smaller value from the larger until they are equal.
    // Correct, but O(max/min) per pair - a value that is a small multiple of the
    // running gcd forces one subtraction per multiple instead of one division.
    private static int SubtractionGcd(int a, int b)
    {
        while (a != b)
        {
            if (a > b)
            {
                a -= b;
            }
            else
            {
                b -= a;
            }
        }

        return a;
    }

    private static int EuclideanGcd(int a, int b) => b == 0 ? a : EuclideanGcd(b, a % b);
}
