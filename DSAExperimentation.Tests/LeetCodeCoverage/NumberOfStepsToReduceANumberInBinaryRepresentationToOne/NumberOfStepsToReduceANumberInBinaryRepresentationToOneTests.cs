namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfStepsToReduceANumberInBinaryRepresentationToOne;

// LeetCode 1404. Number of Steps to Reduce a Number in Binary Representation
// to One: reduces to a single O(n) carry-propagation scan over the binary
// string from the LSB (an even digit is one "divide by 2" step; an odd
// digit is a "+1" step that also sets the carry into the next digit) - the
// same "lighter repo-primitive fit" case RectangleOverlapTests (LC 836)
// already documents; a running carry/step counter needs nothing more than
// two plain ints. See
// NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks.cs for a
// comparison against a brute-force per-step simulation that DOES compose a
// repo primitive (Stack<char>, the same LSB-first binary-add technique
// AddBinaryTests already uses for LC 67).
public sealed partial class NumberOfStepsToReduceANumberInBinaryRepresentationToOneTests
{
    [Fact]
    public void NumSteps_LeetCodeExampleOne_ReturnsSix()
        => Assert.Equal(6, NumSteps("1101"));

    [Fact]
    public void NumSteps_LeetCodeExampleTwo_ReturnsOne()
        => Assert.Equal(1, NumSteps("10"));

    [Fact]
    public void NumSteps_LeetCodeExampleThree_ReturnsZero()
        => Assert.Equal(0, NumSteps("1"));

    [Fact]
    public void NumSteps_CarryCascadesThroughLeadingOnes_CountsEveryStep()
        => Assert.Equal(4, NumSteps("111"));

    private static int NumSteps(string s)
    {
        var steps = 0;
        var carry = 0;
        for (var i = s.Length - 1; i >= 1; i--)
        {
            var digit = (s[i] - '0') + carry;
            if (digit % 2 == 1)
            {
                steps += 2;
                carry = 1;
            }
            else
            {
                steps += 1;
                carry = digit / 2;
            }
        }

        return steps + carry;
    }
}
