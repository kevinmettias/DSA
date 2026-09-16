using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FaultyKeyboardBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - paying for a real reversal on roughly half the keystrokes
// against never reversing at all - so a harness whose arms disagree types two different strings.
// Both arms return the final typed string, which is compared directly. Setup keeps index 0 a
// non-'i' character and makes every even index past it 'i', so the final string is exactly the
// non-'i' keystrokes: the first character plus every odd index. That count is what a reversal arm
// that leaked an 'i' or dropped a character would miss, and the same Length must rebuild it.
public sealed partial class FaultyKeyboardBenchmarksTests
{
    private const int SmallestLength = 200;

    // Index 0 is non-'i', and the odd indices 1..Length-1 are all non-'i' - Length / 2 of them.
    private const int ExpectedFinalLength = (SmallestLength / 2) + 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameKeystrokeRun()
    {
        Assert.Equal(ExpectedFinalLength, BuildHarness().Deque().Length);

        Assert.Equal(BuildHarness().Deque(), BuildHarness().Deque());
    }

    [Fact]
    public void Reversal_EveryOtherKeystrokeIsI_AgreesWithDeque()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Deque(), harness.Reversal());
    }

    [Fact]
    public void Deque_EveryOtherKeystrokeIsI_AgreesWithReversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Reversal(), harness.Deque());
    }

    private static FaultyKeyboardBenchmarks BuildHarness()
    {
        var harness = new FaultyKeyboardBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
