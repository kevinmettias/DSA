using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddTwoNumbersIIBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: a
// bound on the answer that follows from the workload's construction rather than from either arm. Both arms return
// the sum list itself as object? (the node type is internal, CS0050), so the tests read its digits back out.
public sealed partial class AddTwoNumbersIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;

    // Adding two Length-digit lists cannot outgrow a single carry digit, so the sum has at least one digit and at
    // most one more than the operands - the bound either arm's answer has to satisfy.
    [Fact]
    public void BigIntegerConvertAndBack_RandomDigits_GrowsByAtMostOneCarryDigit() =>
        Assert.InRange(DigitsOf(BuildHarness().BigIntegerConvertAndBack()).Length, FewestDigits, DigitsGrownByTheCarry);

    [Fact]
    public void TwoStacksDigitwiseAdd_RandomDigits_GrowsByAtMostOneCarryDigit() =>
        Assert.InRange(DigitsOf(BuildHarness().TwoStacksDigitwiseAdd()).Length, FewestDigits, DigitsGrownByTheCarry);

    private static int[] DigitsOf(object? answer) =>
        LeetCodeWireFormat.FromLinkedList(Assert.IsType<SinglyLinkedListNode<int>>(answer));

    private static AddTwoNumbersIIBenchmarks BuildHarness()
    {
        var harness = new AddTwoNumbersIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
