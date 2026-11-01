using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DoubleANumberRepresentedAsALinkedListBenchmarks (ARCHITECTURE 17.9): a bound
// the workload's shape fixes without either arm. Setup builds one fixed Length-digit list whose first
// digit is nonzero, LC 2816's own guarantee, and doubling a Length-digit number leaves at most one
// carry digit, so the answer is Length or Length + 1 digits long.
public sealed partial class DoubleANumberRepresentedAsALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int DoubledDigitOverflowAllowance = 1;

    [Fact]
    public void BigIntegerConvertDoubleAndBack_TwoHundredDigits_DoublesToAtMostOneExtraDigit() =>
        Assert.InRange(
            DigitsOf(BuildHarness().BigIntegerConvertDoubleAndBack()).Length,
            SmallestLength,
            SmallestLength + DoubledDigitOverflowAllowance);

    private static DoubleANumberRepresentedAsALinkedListBenchmarks BuildHarness()
    {
        var harness = new DoubleANumberRepresentedAsALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The arm hands back a digit list; walking it into its digit sequence counts the digits without
    // changing what the arm computed - the digits are the answer, head to tail.
    private static int[] DigitsOf(SinglyLinkedListNode<int>? answer)
    {
        var digits = new List<int>();

        for (var node = answer; node is not null; node = node.Next)
        {
            digits.Add(node.Value);
        }

        return [.. digits];
    }
}
