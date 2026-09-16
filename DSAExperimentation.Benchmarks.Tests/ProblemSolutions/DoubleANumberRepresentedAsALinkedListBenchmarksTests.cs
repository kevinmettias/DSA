using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DoubleANumberRepresentedAsALinkedListBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - converting the list to a BigInteger and
// unpacking the product's digits against pushing the digits onto this repo's own Stack<int> and
// popping them least-significant-first with a carry - so a harness whose arms disagree is timing two
// different problems. Setup builds one fixed Length-digit list whose first digit is nonzero, LC
// 2816's own guarantee, and neither arm mutates or reverses it, so one harness is safe to read twice
// in either order. Doubling a Length-digit number leaves at most one carry digit, so the answer is
// Length or Length + 1 digits long.
public sealed partial class DoubleANumberRepresentedAsALinkedListBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int DoubledDigitOverflowAllowance = 1;

    [Fact]
    public void Setup_TwoHundredDigits_DoublesToAtMostOneExtraDigitAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var digits = DigitsOf(harness.BigIntegerConvertDoubleAndBack());

        Assert.InRange(digits.Length, SmallestLength, SmallestLength + DoubledDigitOverflowAllowance);
        Assert.Equal(AnswerText.Of(digits), AnswerText.Of(DigitsOf(BuildHarness().BigIntegerConvertDoubleAndBack())));
    }

    [Fact]
    public void BigIntegerConvertDoubleAndBack_SeededDigitList_AgreesWithStackDigitwiseDouble()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(DigitsOf(harness.StackDigitwiseDouble())),
            AnswerText.Of(DigitsOf(harness.BigIntegerConvertDoubleAndBack())));
    }

    [Fact]
    public void StackDigitwiseDouble_SeededDigitList_AgreesWithBigIntegerConvertDoubleAndBack()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(DigitsOf(harness.BigIntegerConvertDoubleAndBack())),
            AnswerText.Of(DigitsOf(harness.StackDigitwiseDouble())));
    }

    private static DoubleANumberRepresentedAsALinkedListBenchmarks BuildHarness()
    {
        var harness = new DoubleANumberRepresentedAsALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // Both arms are declared as returning object because the node type is internal to the solution's
    // own assembly (CS0050); the runtime value they hand back is still that digit list. Walking it
    // into its digit sequence puts both arms into one comparable form without changing what either
    // computed - the digits are the answer, head to tail.
    private static int[] DigitsOf(object? answer)
    {
        var digits = new List<int>();

        for (var node = answer as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            digits.Add(node.Value);
        }

        return [.. digits];
    }
}
