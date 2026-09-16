using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.AddStrings;

// LeetCode 415. Add Strings: sum two base-10 digit strings without converting to a
// numeric type (AddBinarySolution precedent, base 2 there), walking both operands
// least-significant digit first and carrying.
//
// The two strategies differ only in how they undo that reversal - a List<char> the
// caller reverses at the end, or this repo's own Stack<char>, whose LIFO order
// unwinds the digits already in the right order.
internal static class AddStringsSolution
{
    private const int DecimalBase = 10;

    // Accumulate digits least-significant first, then reverse the buffer in place.
    public static string AddByCharArrayReverse(string firstOperand, string secondOperand)
    {
        var chars = new List<char>();
        var walk = new DigitWalk(firstOperand, secondOperand);

        while (walk.HasMore)
        {
            chars.Add(walk.NextDigit());
        }

        chars.Reverse();
        return new string(chars.ToArray());
    }

    // Push digits least-significant first onto Stack<char>; popping yields them
    // most-significant first, so no reversal pass is needed.
    public static string AddByBitStack(string firstOperand, string secondOperand)
    {
        var stack = new DigitStack();
        var walk = new DigitWalk(firstOperand, secondOperand);

        while (walk.HasMore)
        {
            stack.Push(walk.NextDigit());
        }

        var chars = new List<char>();

        while (stack.TryPop(out var digit))
        {
            chars.Add(digit);
        }

        return new string(chars.ToArray());
    }

    // The carry walk itself, shared by both strategies so the only thing they
    // differ in is the digit buffer.
    private sealed class DigitWalk(string firstOperand, string secondOperand)
    {
        private readonly string _firstOperand = firstOperand;
        private readonly string _secondOperand = secondOperand;
        private int _firstIndex = firstOperand.Length - 1;
        private int _secondIndex = secondOperand.Length - 1;
        private int _carry;

        public bool HasMore => _firstIndex >= 0 || _secondIndex >= 0 || _carry > 0;

        public char NextDigit()
        {
            var sum = _carry;

            if (_firstIndex >= 0)
            {
                sum += _firstOperand[_firstIndex--] - '0';
            }

            if (_secondIndex >= 0)
            {
                sum += _secondOperand[_secondIndex--] - '0';
            }

            _carry = sum / DecimalBase;
            return (char)('0' + sum % DecimalBase);
        }
    }
}
