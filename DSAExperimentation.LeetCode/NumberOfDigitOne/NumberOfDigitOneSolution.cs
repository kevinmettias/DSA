using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NumberOfDigitOne;

// LeetCode 233. Number of Digit One: count how many times the digit '1'
// appears across every integer from 1 to n.
//
// CountDigitOneByBruteForceScan is the textbook answer: walk every integer
// 1..n and mod/div-scan its own digits, O(n log n). CountDigitOneByDigitPositionTally
// is the composed answer: this repo's own Stack<int> peels n's decimal
// digits off most-significant-first (the same digit-extraction primitive
// ReverseInteger uses), then a running place-value tally counts every '1'
// across 1..n in O(log n) without ever materializing another number - the
// contribution of each digit position depends only on the digits already
// seen (higherDigits), the digit itself, and the remainder still to come.
internal static class NumberOfDigitOneSolution
{
    // Base of the positional numeral system both strategies decompose upperBound into.
    private const int DecimalBase = 10;

    // The textbook baseline this composition has to justify itself against:
    // a plain nested loop, nothing from this repo.
    public static long CountDigitOneByBruteForceScan(int upperBound)
    {
        long count = 0;

        for (var number = 1; number <= upperBound; number++)
        {
            for (var remaining = number; remaining > 0; remaining /= DecimalBase)
            {
                if (remaining % DecimalBase == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public static long CountDigitOneByDigitPositionTally(int upperBound)
    {
        if (upperBound <= 0)
        {
            return 0;
        }

        var digits = DigitsMostSignificantFirst(upperBound);
        var placeValue = HighestPlaceValue(digits.Count);
        long count = 0;
        var higherDigits = 0L;

        while (digits.TryPop(out var digit))
        {
            count += AccumulateDigitOnes(digit, upperBound, ref placeValue, ref higherDigits);
        }

        return count;
    }

    // Pushes upperBound's decimal digits onto this repo's own Stack<int> least
    // significant first, so popping them yields the most significant digit first.
    private static DigitStack DigitsMostSignificantFirst(int upperBound)
    {
        var digits = new DigitStack();

        for (var remaining = upperBound; remaining > 0; remaining /= DecimalBase)
        {
            digits.Push(remaining % DecimalBase);
        }

        return digits;
    }

    // The place value of the digit that pops first (the most significant one):
    // DecimalBase raised to one less than the digit count.
    private static long HighestPlaceValue(int digitCount)
    {
        var placeValue = 1L;

        for (var i = 1; i < digitCount; i++)
        {
            placeValue *= DecimalBase;
        }

        return placeValue;
    }

    // Folds one popped digit into the running ones-count, then advances
    // placeValue and higherDigits to the next (more significant) digit
    // position - the self-contained per-digit step of the place-value tally
    // above.
    private static long AccumulateDigitOnes(int digit, long upperBound, ref long placeValue, ref long higherDigits)
    {
        var lowerRemainder = upperBound % placeValue;
        var delta = digit switch
        {
            0 => higherDigits * placeValue,
            1 => (higherDigits * placeValue) + lowerRemainder + 1,
            _ => (higherDigits + 1) * placeValue,
        };

        higherDigits = (higherDigits * DecimalBase) + digit;
        placeValue /= DecimalBase;
        return delta;
    }
}
