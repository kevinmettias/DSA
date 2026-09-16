using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddBinaryBenchmarks (ARCHITECTURE 17.9): its two arms differ only in how they undo the
// least-significant-first digit walk - a List<char> reversed at the end, or this repo's own Stack<char> - so a
// harness whose arms disagree is timing two different problems. Setup hands both arms all-ones operands, which
// is the decisive case the class comment names: every digit position carries, so the result always grows by one
// bit. Two Length-bit all-ones numbers sum to a number whose top bit is set and whose bottom bit is clear, so
// the answer is Length ones followed by a zero - asserted literally rather than against whichever arm ran first.
public sealed partial class AddBinaryBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int GrownLength = SmallestLength + 1;

    [Fact]
    public void Setup_AllOnesOperands_RebuildsTheSameCarriedSum()
    {
        Assert.Equal(ExpectedCarriedSum, BuildHarness().CharArrayReverse());
        Assert.Equal(
            AnswerText.Of(BuildHarness().StackBits()),
            AnswerText.Of(BuildHarness().StackBits()));
    }

    [Fact]
    public void CharArrayReverse_AllOnesOperands_GrowsTheSumByExactlyOneBit() =>
        Assert.Equal(GrownLength, BuildHarness().CharArrayReverse().Length);

    [Fact]
    public void CharArrayReverse_AllOnesOperands_AgreesWithStackBits()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StackBits()), AnswerText.Of(harness.CharArrayReverse()));
    }

    [Fact]
    public void StackBits_AllOnesOperands_AgreesWithCharArrayReverse()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.CharArrayReverse()), AnswerText.Of(harness.StackBits()));
    }

    // Both arms are handed the same all-ones pair, so the sum is fixed by arithmetic alone: n ones is 2^n - 1,
    // twice that is 2^(n+1) - 2, whose binary form is n ones with a zero in the last place.
    private static string ExpectedCarriedSum => new string('1', SmallestLength) + "0";

    private static AddBinaryBenchmarks BuildHarness()
    {
        var harness = new AddBinaryBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
