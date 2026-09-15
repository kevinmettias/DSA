using BitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.AddBinary;

// LeetCode 67. Add Binary: sum two binary strings without converting to a numeric
// type, walking both operands least-significant digit first and carrying.
//
// The two strategies differ only in how they undo that reversal - a List<char> the
// caller reverses at the end, or this repo's own Stack<char>, whose LIFO order
// unwinds the digits already in the right order.
internal static class AddBinarySolution
{
    private const int BinaryBase = 2;

    // Accumulate digits least-significant first, then reverse the buffer in place.
    public static string AddByCharArrayReverse(string a, string b)
    {
        var chars = new List<char>();
        var walk = new DigitWalk(a, b);

        while (walk.HasMore)
        {
            chars.Add(walk.NextBit());
        }

        chars.Reverse();
        return new string(chars.ToArray());
    }

    // Push digits least-significant first onto Stack<char>; popping yields them
    // most-significant first, so no reversal pass is needed.
    public static string AddByBitStack(string a, string b)
    {
        var stack = new BitStack();
        var walk = new DigitWalk(a, b);

        while (walk.HasMore)
        {
            stack.Push(walk.NextBit());
        }

        var chars = new List<char>();

        while (stack.TryPop(out var bit))
        {
            chars.Add(bit);
        }

        return new string(chars.ToArray());
    }

    // The carry walk itself, shared by both strategies so the only thing they
    // differ in is the digit buffer.
    private sealed class DigitWalk(string a, string b)
    {
        private readonly string _a = a;
        private readonly string _b = b;
        private int _i = a.Length - 1;
        private int _j = b.Length - 1;
        private int _carry;

        public bool HasMore => _i >= 0 || _j >= 0 || _carry > 0;

        public char NextBit()
        {
            var sum = _carry;

            if (_i >= 0)
            {
                sum += _a[_i--] - '0';
            }

            if (_j >= 0)
            {
                sum += _b[_j--] - '0';
            }

            _carry = sum / BinaryBase;
            return (char)('0' + sum % BinaryBase);
        }
    }
}
