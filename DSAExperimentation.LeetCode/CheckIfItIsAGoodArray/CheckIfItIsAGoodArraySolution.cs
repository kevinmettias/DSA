namespace DSAExperimentation.LeetCode.CheckIfItIsAGoodArray;

// LeetCode 1250. Check If It Is a Good Array: decide whether some
// integer-coefficient combination of nums sums to 1.
//
// Bezout's identity collapses the whole problem to "is gcd(nums) == 1", so the
// array itself is one running fold over a pairwise gcd and the only strategy
// choice left is how that pairwise step is computed - repeated subtraction (the
// textbook first gcd algorithm, O(max(a,b)/min(a,b)) per pair) against the
// modulo-based Euclidean algorithm (O(log min(a,b)) per pair).
//
// No repo container or algorithm primitive applies to a reduction over a
// running integer: the private Euclidean helper below is the same one
// WaterAndJugProblemSolution and XOfAKindInADeckOfCardsSolution already keep
// inline rather than promoting to a shared production type.
internal static class CheckIfItIsAGoodArraySolution
{
    // The textbook answer: the subtraction form of Euclid's algorithm, folded
    // over the array with no short-circuit. Deliberately written without this
    // repo's primitives - it is the arm the Euclidean fold below has to justify
    // itself against, and its cost is what makes the comparison visible.
    public static bool IsGoodArrayBySubtractionGcd(int[] nums)
    {
        var gcd = nums[0];

        foreach (var value in nums)
        {
            gcd = SubtractionGcd(gcd, value);
        }

        return gcd == 1;
    }

    // Repeatedly subtract the smaller value from the larger until they are
    // equal. Correct, but O(max/min) per pair - a value that is a small multiple
    // of the running gcd forces one subtraction per multiple instead of one
    // division.
    private static int SubtractionGcd(int firstValue, int secondValue)
    {
        while (firstValue != secondValue)
        {
            if (firstValue > secondValue)
            {
                firstValue -= secondValue;
            }
            else
            {
                secondValue -= firstValue;
            }
        }

        return firstValue;
    }

    // The modulo form, short-circuiting the moment the running gcd reaches 1 -
    // once it does, no later element can move it, so the remaining elements
    // cannot change the answer.
    public static bool IsGoodArrayByEuclideanGcd(int[] nums)
    {
        var gcd = nums[0];

        foreach (var value in nums)
        {
            gcd = EuclideanGcd(gcd, value);

            if (gcd == 1)
            {
                return true;
            }
        }

        return gcd == 1;
    }

    private static int EuclideanGcd(int firstValue, int secondValue) =>
        secondValue == 0 ? firstValue : EuclideanGcd(secondValue, firstValue % secondValue);
}
