using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.AddDigits;

// LeetCode 258. Add Digits: repeatedly sum a number's digits until only one digit
// remains (the digital root).
//
// The two strategies differ only in how each summation pass holds the digits it is
// about to add: plain arithmetic with no intermediate storage, or pushing them onto
// this repo's own LIFO Stack<T> and popping them all back off to sum.
internal static class AddDigitsSolution
{
    private const int DecimalBase = 10;

    // The textbook answer: modulo/divide straight into a running total, no
    // intermediate storage. Deliberately written without this repo's primitives -
    // it is the arm the Stack-based strategy has to justify itself against.
    public static int AddDigitsByArithmetic(int num)
    {
        while (num >= DecimalBase)
        {
            var sum = 0;
            var remaining = num;

            while (remaining > 0)
            {
                sum += remaining % DecimalBase;
                remaining /= DecimalBase;
            }

            num = sum;
        }

        return num;
    }

    // Each pass pushes the digits onto Stack<T> and pops them all back off to sum -
    // the same explicit digit-extraction shape ReverseIntegerSolution/PlusOneSolution
    // use their DigitStack for.
    public static int AddDigitsByStack(int num)
    {
        while (num >= DecimalBase)
        {
            var digits = new DigitStack();
            var remaining = num;

            while (remaining > 0)
            {
                digits.Push(remaining % DecimalBase);
                remaining /= DecimalBase;
            }

            num = 0;

            while (digits.TryPop(out var digit))
            {
                num += digit;
            }
        }

        return num;
    }
}
