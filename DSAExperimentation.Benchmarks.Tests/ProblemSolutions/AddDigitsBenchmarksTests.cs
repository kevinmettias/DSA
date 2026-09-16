using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddDigitsBenchmarks (ARCHITECTURE 17.9): both arms are AddDigitsSolution's, differing
// only in how each summation pass holds the digits it is about to add - plain arithmetic or this repo's own
// LIFO Stack<T> - so a harness whose arms disagree is timing two different problems. The class has no
// [GlobalSetup]: the workload is the parameter itself, so the harness is constructed with [Params]' own values
// and the arms are called directly. The digital root is derived here by repeated digit summing rather than
// trusting either arm, so the expected answer is an independent oracle and not a restatement of an arm.
public sealed partial class AddDigitsBenchmarksTests
{
    private const int SmallestValue = 999_999;
    private const int LargestValue = int.MaxValue;

    [Fact]
    public void Arithmetic_AllNineDigits_ReturnsTheDigitalRoot() =>
        Assert.Equal(DigitalRoot(SmallestValue), Harness(SmallestValue).Arithmetic());

    [Fact]
    public void StackDigits_AllNineDigits_AgreesWithArithmetic()
    {
        var harness = Harness(SmallestValue);

        Assert.Equal(harness.Arithmetic(), harness.StackDigits());
    }

    [Fact]
    public void Arithmetic_MaximumValue_ReturnsTheDigitalRoot() =>
        Assert.Equal(DigitalRoot(LargestValue), Harness(LargestValue).Arithmetic());

    [Fact]
    public void StackDigits_MaximumValue_AgreesWithArithmetic()
    {
        var harness = Harness(LargestValue);

        Assert.Equal(harness.Arithmetic(), harness.StackDigits());
    }

    // The definition itself - sum the digits, repeat while more than one remains - so the expected value comes
    // from the problem statement rather than from either arm.
    private static int DigitalRoot(int value)
    {
        var remaining = Math.Abs(value);

        while (remaining >= DecimalBase)
        {
            var sum = 0;

            while (remaining > 0)
            {
                sum += remaining % DecimalBase;
                remaining /= DecimalBase;
            }

            remaining = sum;
        }

        return remaining;
    }

    private static AddDigitsBenchmarks Harness(int value) => new() { Value = value };

    private static int DecimalBase => 10;
}
