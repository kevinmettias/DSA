using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddTwoNumbersIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// AddTwoNumbersIISolution's, the same methods AddTwoNumbersIITests proves correct - a BigInteger conversion
// and back against pushing both lists onto this repo's own Stack<int> and popping them in lockstep - so a
// harness whose arms disagree is timing two different problems. Both arms are declared as returning int
// because the node type is internal (CS0050) and CountDigits exists to give the public [Benchmark] a public
// return value, so what can be compared here is the digit count of the sum, not the sum's digits.
public sealed partial class AddTwoNumbersIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int FewestDigits = 1;
    private const int DigitsGrownByTheCarry = SmallestLength + 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDigitCount()
    {
        Assert.Equal(BuildHarness().BigIntegerConvertAndBack(), BuildHarness().BigIntegerConvertAndBack());
        Assert.Equal(BuildHarness().TwoStacksDigitwiseAdd(), BuildHarness().TwoStacksDigitwiseAdd());
    }

    // Adding two Length-digit lists cannot outgrow a single carry digit, so the count is at least one and at
    // most one more than the operands - the bound either arm's answer has to satisfy.
    [Fact]
    public void BigIntegerConvertAndBack_RandomDigits_AgreesWithTwoStacksDigitwiseAdd()
    {
        var harness = BuildHarness();
        var count = harness.TwoStacksDigitwiseAdd();

        Assert.InRange(count, FewestDigits, DigitsGrownByTheCarry);
        Assert.Equal(count, harness.BigIntegerConvertAndBack());
    }

    [Fact]
    public void TwoStacksDigitwiseAdd_RandomDigits_AgreesWithBigIntegerConvertAndBack()
    {
        var harness = BuildHarness();
        var count = harness.BigIntegerConvertAndBack();

        Assert.InRange(count, FewestDigits, DigitsGrownByTheCarry);
        Assert.Equal(count, harness.TwoStacksDigitwiseAdd());
    }

    private static AddTwoNumbersIIBenchmarks BuildHarness()
    {
        var harness = new AddTwoNumbersIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
