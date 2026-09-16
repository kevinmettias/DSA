using BitStack = System.Collections.Generic.Stack<char>;

namespace DSAExperimentation.LeetCode.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

// LeetCode 1404. Number of Steps to Reduce a Number in Binary Representation to One:
// count the steps that take the binary string to "1", halving it when it is even and
// adding one when it is odd.
//
// Both strategies count the same steps and differ only in whether they materialize the
// intermediate numbers: one performs every "+1" as a real binary addition, the other
// reads the answer off a single right-to-left pass by carrying.
internal static class NumberOfStepsToReduceANumberInBinaryRepresentationToOneSolution
{
    private const int BinaryBase = 2;
    private const int StepsForOddDigit = 2;
    private const string SingleBitOne = "1";

    // The textbook answer: actually run the reduction. An even number drops its last
    // bit; an odd number is incremented by a full LSB-first binary addition, rebuilding
    // the whole remaining string. Deliberately written with BCL types only - it is the
    // arm the single-pass scan below has to justify itself against.
    public static int CountStepsByStackAddSimulation(string binaryString)
    {
        var current = binaryString;
        var steps = 0;

        while (current != SingleBitOne)
        {
            var isEven = current[^1] == '0';
            current = isEven ? WithoutLastBit(current) : AddOne(current);
            steps++;
        }

        return steps;
    }

    // The number halved: an even binary number just drops its last bit.
    private static string WithoutLastBit(string current) => current[..^1];

    // Increment a binary string, pushing the sum digits least-significant first so that
    // popping yields them most-significant first.
    private static string AddOne(string binaryString)
    {
        var stack = new BitStack();
        var cursorIndex = binaryString.Length - 1;
        var carry = 1;

        while (cursorIndex >= 0 || carry > 0)
        {
            var sum = carry + (cursorIndex >= 0 ? NextDigit(binaryString, ref cursorIndex) : 0);
            stack.Push((char)('0' + (sum % BinaryBase)));
            carry = sum / BinaryBase;
        }

        var chars = new List<char>();

        while (stack.TryPop(out var bit))
        {
            chars.Add(bit);
        }

        return new string(chars.ToArray());
    }

    // The digit at the cursor, the read consuming it: the addition walks a
    // least-significant digit first.
    private static int NextDigit(string binaryString, ref int cursorIndex) => binaryString[cursorIndex--] - '0';

    // One right-to-left pass with a running carry. Every digit above the leading bit
    // costs one halving step, plus one more when the digit (with the carry into it) is
    // odd, which also carries into the next digit; a carry surviving the leading bit is
    // the final halving.
    public static int CountStepsByCarryPropagationScan(string binaryString)
    {
        var steps = 0;
        var carry = 0;

        for (var i = binaryString.Length - 1; i >= 1; i--)
        {
            var digit = (binaryString[i] - '0') + carry;

            if (digit % BinaryBase == 1)
            {
                steps += StepsForOddDigit;
                carry = 1;
            }
            else
            {
                steps += 1;
                carry = digit / BinaryBase;
            }
        }

        return steps + carry;
    }
}
