using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddTwoNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are AddTwoNumbersSolution's,
// the same methods AddTwoNumbersTests proves correct - a BigInteger conversion and back against a single
// digit-wise walk with a running carry - so a harness whose arms disagree is timing two different problems.
// Both arms are declared as returning object (the node type is internal, CS0050), so the tests read the digit
// sequence off the returned list and compare that, which is the answer LC 2 actually asks for. Setup draws
// both operands from one fixed seed, so the same Length must rebuild the same two lists.
public sealed partial class AddTwoNumbersBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDigitLists()
    {
        Assert.Equal(
            AnswerText.Of(DigitsOf(BuildHarness().DigitwiseListWalk())),
            AnswerText.Of(DigitsOf(BuildHarness().DigitwiseListWalk())));
        Assert.Equal(
            AnswerText.Of(DigitsOf(BuildHarness().BigIntegerConvertAndBack())),
            AnswerText.Of(DigitsOf(BuildHarness().BigIntegerConvertAndBack())));
    }

    // Adding two Length-digit lists cannot outgrow a single carry digit, so the sum is as long as its operands
    // or one digit longer - the bound the answer has to satisfy whichever arm produced it.
    [Fact]
    public void BigIntegerConvertAndBack_RandomDigits_AgreesWithDigitwiseListWalk()
    {
        var harness = BuildHarness();
        var sum = DigitsOf(harness.DigitwiseListWalk());

        Assert.InRange(sum.Length, FewestDigits, DigitsGrownByTheCarry);
        Assert.Equal(AnswerText.Of(sum), AnswerText.Of(DigitsOf(harness.BigIntegerConvertAndBack())));
    }

    [Fact]
    public void DigitwiseListWalk_RandomDigits_AgreesWithBigIntegerConvertAndBack()
    {
        var harness = BuildHarness();
        var sum = DigitsOf(harness.BigIntegerConvertAndBack());

        Assert.InRange(sum.Length, FewestDigits, DigitsGrownByTheCarry);
        Assert.Equal(AnswerText.Of(sum), AnswerText.Of(DigitsOf(harness.DigitwiseListWalk())));
    }

    // Both arms hand back the internal node type through an object, so walking it into its digit sequence puts
    // them into one comparable form without changing what either computed - the digits are the answer.
    private static int[] DigitsOf(object? answer)
    {
        var digits = new List<int>();

        for (var node = answer as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            digits.Add(node.Value);
        }

        return [.. digits];
    }

    private static AddTwoNumbersBenchmarks BuildHarness()
    {
        var harness = new AddTwoNumbersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
