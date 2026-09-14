namespace DSAExperimentation.LeetCode.FindGreatestCommonDivisorOfArray;

// LeetCode 1979. Find Greatest Common Divisor of Array: the greatest common
// divisor of the array's smallest and largest elements.
//
// Locating those two elements is one O(n) scan with no interesting container to
// compose, so the only strategy choice left is how the single Gcd(min, max) call
// is computed - repeated subtraction (the textbook first algorithm, O(max/min)
// here) against the modulo-based Euclidean algorithm (O(log min)). That is the
// same pairwise contrast CheckIfItIsAGoodArraySolution draws for its array-wide
// gcd fold, and the private Euclidean helper below is the one that solution and
// XOfAKindInADeckOfCardsSolution already keep inline rather than promoting to a
// shared production type - a reduction over a running integer has no repo
// container or algorithm primitive to reach for.
internal static class FindGreatestCommonDivisorOfArraySolution
{
    // The textbook answer: subtract the smaller value from the larger until they
    // meet. Deliberately written without this repo's primitives - it is the arm
    // the Euclidean strategy below has to justify itself against, and its
    // O(max/min) cost is what makes the comparison visible.
    public static int FindGcdBySubtraction(int[] nums)
    {
        var (min, max) = MinAndMax(nums);

        return SubtractionGcd(min, max);
    }

    // The modulo form: each step replaces the pair with (b, a % b), so the values
    // shrink geometrically instead of by one multiple at a time.
    public static int FindGcdByEuclidean(int[] nums)
    {
        var (min, max) = MinAndMax(nums);

        return EuclideanGcd(min, max);
    }

    // One pass for both ends. A single-element array has min == max, and gcd(x, x)
    // is x, which is the answer LeetCode expects there.
    private static (int Min, int Max) MinAndMax(int[] nums)
    {
        var min = nums[0];
        var max = nums[0];

        foreach (var num in nums)
        {
            min = Math.Min(min, num);
            max = Math.Max(max, num);
        }

        return (min, max);
    }

    // Correct, but O(max/min): a small min forces one subtraction per unit of the
    // gap instead of one division. Terminates because every value here is at
    // least 1 (LC 1979's own bound), so the pair strictly decreases.
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
