namespace DSAExperimentation.Algorithms.NumberTheory;

// value^exponent mod modulus by repeated squaring, O(log exponent) - the algorithm. Which modulus a
// problem reports in is not part of it: Domain/Modular/ModularArithmetic fixes LeetCode's
// 1_000_000_007 and calls this, the split 17.6 draws between a classic algorithm and the convention
// that pins its content.
//
// long only, deliberately not generic: every step squares a value below `modulus`, so the modulus
// must keep (modulus - 1)^2 inside T. For long that is moduli up to about 3.03e9; for int it would
// be 46,341, below the moduli callers use. A generic signature would accept T = int and overflow.
//
// A negative value keeps a negative remainder (C#'s %), and exponent 0 - or any exponent <= 0 -
// gives 1 % modulus: exactly what ModularArithmetic.Power returned before it delegated here.
internal static class ModularPower
{
    public static long Of(long value, long exponent, long modulus)
    {
        var result = 1 % modulus;
        value %= modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % modulus;
            }

            value = value * value % modulus;
            exponent >>= 1;
        }

        return result;
    }
}
