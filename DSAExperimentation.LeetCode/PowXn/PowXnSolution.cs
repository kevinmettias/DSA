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
    // The textbook baseline: multiply the base into the result once per unit of
    // the exponent counter. Deliberately written without this repo's primitives -
    // it is the arm the squaring strategy below has to justify itself against.
    public static double PowByRepeatedMultiplication(double baseValue, int power)
    {
        long exponent = power;

        if (exponent < 0)
        {
            baseValue = 1 / baseValue;
            exponent = -exponent;
        }

        var result = 1.0;

        for (var i = 0L; i < exponent; i++)
        {
            result *= baseValue;
        }

        return result;
    }

    // Halve the exponent each step instead of decrementing it, squaring the
    // base to compensate - the same quantity in O(log |n|) multiplications.
    public static double PowByExponentiationBySquaring(double baseValue, int power)
    {
        long exponent = power;

        if (exponent < 0)
        {
            baseValue = 1 / baseValue;
            exponent = -exponent;
        }

        var result = 1.0;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= baseValue;
            }

            baseValue *= baseValue;
            exponent >>= 1;
        }

        return result;
    }
}
