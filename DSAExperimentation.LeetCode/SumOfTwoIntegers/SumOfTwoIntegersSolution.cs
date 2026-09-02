namespace DSAExperimentation.LeetCode.SumOfTwoIntegers;

// LeetCode 371. Sum of Two Integers: the problem's own constraint (no + or -) rules
// out every arithmetic operator, and no data structure applies either - this is a
// pure bitwise carry-propagation loop, the same "no stronger reusable primitive"
// shape MaximumSubarrayBenchmarks/GasStation/BestTimeToBuyAndSellStock already
// establish.
//
// BuiltInAddition is the textbook baseline - "what you would write without this
// problem's constraint" (17.5) - and GetSumByBitwiseCarryLoop is the
// actually-compliant algorithm: XOR gives the carry-less sum of each bit pair,
// AND-then-shift gives the carry to fold back in next iteration, repeat until
// no carry remains.
internal static class SumOfTwoIntegersSolution
{
    // The baseline: plain integer addition. Violates the problem's own
    // no-arithmetic-operator constraint by construction, which is exactly why it
    // is the thing GetSumByBitwiseCarryLoop is measured against rather than a
    // valid submission on its own.
    public static int GetSumByBuiltInAddition(int a, int b) => a + b;

    public static int GetSumByBitwiseCarryLoop(int a, int b)
    {
        while (b != 0)
        {
            var carry = (a & b) << 1;
            a ^= b;
            b = carry;
        }

        return a;
    }
}
