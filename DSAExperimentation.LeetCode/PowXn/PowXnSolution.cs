namespace DSAExperimentation.LeetCode.PowXn;

// LeetCode 50. Pow(x, n): raise a double to an integer power, positive or
// negative, without the BCL's own Math.Pow.
//
// Two strategies over the same running scalar and exponent counter: repeated
// multiplication (the textbook O(|n|) baseline) and exponentiation by
// squaring (O(log |n|)). No repo container or algorithm primitive applies -
// there is nothing to compose over one double and one exponent counter, the
// same "lighter repo-primitive fit" case as Power of Two's bit trick.
internal static class PowXnSolution
{
    // The textbook baseline: multiply by x once per unit of exponent.
    // Deliberately written without this repo's primitives - it is the arm the
    // squaring strategy below has to justify itself against.
    public static double PowByRepeatedMultiplication(double x, int n)
    {
        long exponent = n;

        if (exponent < 0)
        {
            x = 1 / x;
            exponent = -exponent;
        }

        var result = 1.0;

        for (var i = 0L; i < exponent; i++)
        {
            result *= x;
        }

        return result;
    }

    // Halve the exponent each step instead of decrementing it, squaring the
    // base to compensate - the same quantity in O(log |n|) multiplications.
    public static double PowByExponentiationBySquaring(double x, int n)
    {
        long exponent = n;

        if (exponent < 0)
        {
            x = 1 / x;
            exponent = -exponent;
        }

        var result = 1.0;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= x;
            }

            x *= x;
            exponent >>= 1;
        }

        return result;
    }
}
