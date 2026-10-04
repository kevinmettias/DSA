namespace DSAExperimentation.LeetCode.SumOfTwoIntegers;

// LeetCode 371. Sum of Two Integers: the problem's own constraint (no + or -) rules
// out every arithmetic operator, and no data structure applies either - this is
// pure bitwise carry propagation, the same "no stronger reusable primitive" shape
// MaximumSubarrayBenchmarks/GasStation/BestTimeToBuyAndSellStock already establish.
//
// Both strategies are compliant and differ in how far a carry moves per step.
// GetSumByRippleCarryAdder, the baseline, is the circuit written out: one bit
// position at a time, 32 steps whatever the addends. GetSumByBitwiseCarryLoop works
// on every position at once: XOR gives the carry-less sum of each bit pair,
// AND-then-shift gives the carries to fold back in next iteration, repeat until no
// carry remains - as many steps as the longest carry chain.
internal static class SumOfTwoIntegersSolution
{
    // A ripple-carry adder: a one-bit mask walks the 32 positions, low to high, and
    // shifting it is the loop's only step, so not even the loop counter adds. At each
    // position the sum bit is the XOR of the two addend bits and the incoming carry,
    // and the outgoing carry is set when at least two of those three are. Two's
    // complement makes the same 32 positions right for negative addends, and the
    // carry out of the top bit falls off the shift, as int addition drops it.
    public static int GetSumByRippleCarryAdder(int firstAddend, int secondAddend)
    {
        var sum = 0;
        var carry = 0;

        for (var position = 1; position != 0; position <<= 1)
        {
            var firstBit = firstAddend & position;
            var secondBit = secondAddend & position;

            sum |= firstBit ^ secondBit ^ carry;
            carry = ((firstBit & secondBit) | (carry & (firstBit ^ secondBit))) << 1;
        }

        return sum;
    }

    public static int GetSumByBitwiseCarryLoop(int firstAddend, int secondAddend)
    {
        while (secondAddend != 0)
        {
            var carry = (firstAddend & secondAddend) << 1;
            firstAddend ^= secondAddend;
            secondAddend = carry;
        }

        return firstAddend;
    }
}
