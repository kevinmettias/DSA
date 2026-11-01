using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddTwoNumbersBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other on the seeded operands, and this bounds the baseline's sum by what addition itself allows - a property
// no comparison between arms can establish. The baseline returns the sum list, so the test reads the digit
// sequence off the returned list, which is the answer LC 2 actually asks for.
public sealed partial class AddTwoNumbersBenchmarksTests
{
    private const int SmallestLength = 10;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;

    // Adding two Length-digit lists cannot outgrow a single carry digit, so the sum is as long as its operands
    // or one digit longer - the bound the answer has to satisfy whichever arm produced it.
    [Fact]
    public void BigIntegerConvertAndBack_RandomDigits_GrowsByAtMostOneCarryDigit() =>
        Assert.InRange(DigitsOf(BuildHarness().BigIntegerConvertAndBack()).Length, FewestDigits, DigitsGrownByTheCarry);

    // The arm hands back a linked list, so walking it into its digit sequence reads the answer off without
    // changing what it computed - the digits are the answer.
    private static int[] DigitsOf(SinglyLinkedListNode<int>? answer)
    {
        var digits = new List<int>();

        for (var node = answer; node is not null; node = node.Next)
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
