using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BackspaceStringCompareBenchmarks (ARCHITECTURE 17.9): its two arms are
// BackspaceStringCompareSolution's competing strategies for the same question - the BCL's Stack<char> against this
// repo's own DynamicArray-backed Stack<char>, both replaying the same keystrokes - so a harness whose arms
// disagree is replaying two different pairs of strings.
//
// Both texts are built from the SAME seed on purpose, so each arm is forced through its full replay of both
// strings instead of short-circuiting on an early mismatch. That makes the verdicts TRUE by construction, and the
// agreement it witnesses is therefore weak: a rigged always-true arm would agree too. The tests below assert the
// decisive literal (identical seed, so equal) alongside the agreement, which is the strongest honest thing this
// fixture admits; strengthening it further would mean changing what the benchmark feeds its arms.
public sealed partial class BackspaceStringCompareBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] keystroke-run lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsTypedTextEqualByBclStack(),
            BuildHarness().IsTypedTextEqualByBclStack());

    [Fact]
    public void IsTypedTextEqualByBclStack_SameSeedTexts_AgreesWithStackReplay()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsTypedTextEqualByBclStack());
        Assert.Equal(harness.IsTypedTextEqualByStackReplay(), harness.IsTypedTextEqualByBclStack());
    }

    [Fact]
    public void IsTypedTextEqualByStackReplay_SameSeedTexts_AgreesWithBclStack()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsTypedTextEqualByStackReplay());
        Assert.Equal(harness.IsTypedTextEqualByBclStack(), harness.IsTypedTextEqualByStackReplay());
    }

    private static BackspaceStringCompareBenchmarks BuildHarness()
    {
        var harness = new BackspaceStringCompareBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
