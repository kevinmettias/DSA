using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOne;

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
// once for the whole array - which is what makes it visible. The modulo step is
// the library's own Euclid, GreatestCommonDivisor.Of, as in those two; the
// subtraction step stays in this file because the baseline arm runs on it
// (ARCHITECTURE 17.5).
[LeetCodeQuestion("minimum-number-of-operations-to-make-all-array-elements-equal-to-1", LeetCodeAnswerOrder.AsWritten)]
internal static class MinimumNumberOfOperationsToMakeAllArrayElementsEqualToOneSolution
{
    // The textbook answer: the subtraction form of Euclid's algorithm driving the
    // window scan. Deliberately written without this repo's primitives - it is
    // the arm the Euclidean scan below has to justify itself against.
    public static int MinOperationsBySubtractionGcd(int[] nums)
    {
        var ones = CountOnes(nums);
        var shortestWindow = ShortestGcdWindow<SubtractionGcd>(nums);

        return ones > 0
            ? WalkOneAlongCost(nums.Length, ones)
            : SpreadCost(nums.Length, shortestWindow);
    }

    // The modulo form of the same scan: each step replaces the running pair with
    // (b, a % b), so the values shrink geometrically instead of by one multiple
    // of the smaller at a time.
    public static int MinOperationsByEuclideanGcd(int[] nums)
    {
        var ones = CountOnes(nums);
        var shortestWindow = ShortestGcdWindow<EuclideanGcd>(nums);

        return ones > 0
            ? WalkOneAlongCost(nums.Length, ones)
            : SpreadCost(nums.Length, shortestWindow);
    }

    // The one step the two arms compute differently: the greatest common divisor of two
    // values. Naming it is what gives a gcd step somewhere to state its contract - a
    // divisor of both, and the largest such - which a bare int-by-int-by-int callable
    // had nowhere to write down.
    private interface IPairwiseGcd
    {
        static abstract int Of(int left, int right);
    }

    // The window scan both arms share: grow a window from each start until its running
    // gcd reaches 1, and keep the shortest one found. Windows of length 1 are
    // deliberately not considered: an element that is already 1 is handled by the ones
    // count, and no other single element has gcd 1 with itself. Which pairwise gcd the
    // running value folds through is the one thing the two arms differ in, so it
    // arrives as TGcd - a struct type argument, so each arm gets its own copy of the
    // scan with its step inlined, where a gcd held as an interface made a dispatched
    // call for every element of every window.
    private static int? ShortestGcdWindow<TGcd>(int[] nums)
        where TGcd : struct, IPairwiseGcd
    {
        int? best = null;

        for (var start = 0; start < nums.Length; start++)
        {
            var running = nums[start];

            for (var end = start + 1; end < nums.Length; end++)
            {
                running = TGcd.Of(running, nums[end]);

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

    // Building one 1 costs (window - 1) operations; spreading it over the array
    // costs (n - 1) more. No window at all means no 1 is reachable, which is
    // LeetCode's -1. The window helpers report that as null rather than sharing
    // this tier's answer sentinel, the same reason HammingSearch returns int?
    // instead of exporting an Unreachable constant (ARCHITECTURE.md 17.9).
    private static int SpreadCost(int length, int? shortestWindow)
        => shortestWindow is null ? LeetCodeAnswer.None : CollapseThenSpreadCost(length, shortestWindow.Value);

    // Collapsing the window itself costs (window - 1) operations - it takes that
    // many steps to leave a single 1 standing - and spreading that 1 over the
    // remaining elements costs (length - 1) more.
    private static int CollapseThenSpreadCost(int length, int window) => (window - 1) + (length - 1);

    // With a 1 already in the array it can simply be walked from neighbour to
    // neighbour, so every other element costs exactly one operation.
    private static int WalkOneAlongCost(int total, int ones) => total - ones;

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

    // Repeatedly subtract the smaller value from the larger until they are equal.
    // Correct, but O(max/min) per pair - a value that is a small multiple of the
    // running gcd forces one subtraction per multiple instead of one division.
    private readonly struct SubtractionGcd : IPairwiseGcd
    {
        public static int Of(int left, int right)
        {
            while (left != right)
            {
                if (left > right)
                {
                    left -= right;
                }
                else
                {
                    right -= left;
                }
            }

            return left;
        }
    }

    // The modulo form of the same step, GreatestCommonDivisor.Of: each round
    // replaces the pair with (right, left % right), so the values shrink
    // geometrically instead of by one multiple of the smaller at a time.
    private readonly struct EuclideanGcd : IPairwiseGcd
    {
        public static int Of(int left, int right) => GreatestCommonDivisor.Of(left, right);
    }
}
