using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FaultyKeyboardBenchmarks (ARCHITECTURE 17.9): an expected length known from
// the workload's shape. Setup keeps index 0 a non-'i' character and makes every even index past it
// 'i', so the final string is exactly the non-'i' keystrokes: the first character plus every odd
// index. That count is what an arm that leaked an 'i' or dropped a character would miss.
public sealed partial class FaultyKeyboardBenchmarksTests
{
    private const int SmallestLength = 10;

    // Index 0 is non-'i', and the odd indices 1..Length-1 are all non-'i' - Length / 2 of them.
    private const int ExpectedFinalLength = (SmallestLength / 2) + 1;

    [Fact]
    public void Deque_EveryOtherKeystrokeIsI_KeepsOnlyTheNonIKeystrokes() =>
        Assert.Equal(ExpectedFinalLength, BuildHarness().Deque().Length);

    private static FaultyKeyboardBenchmarks BuildHarness()
    {
        var harness = new FaultyKeyboardBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
