using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BackspaceStringCompareBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin. Both texts are built from the SAME seed on purpose, so each arm is forced through its full replay of both
// strings instead of short-circuiting on an early mismatch. That makes the verdicts TRUE by construction, and the
// agreement the generic check witnesses is therefore weak: a rigged always-true arm would agree too. The test
// below asserts the decisive literal (identical seed, so equal), which is the strongest honest thing this fixture
// admits; strengthening it further would mean changing what the benchmark feeds its arms.
public sealed partial class BackspaceStringCompareBenchmarksTests
{
    // The smaller of Setup's [Params(20, 200)] keystroke-run lengths.
    private const int SmallestLength = 20;

    [Fact]
    public void IsTypedTextEqualByBclStack_SameSeedTexts_ReturnsTrue() =>
        Assert.True(BuildHarness().IsTypedTextEqualByBclStack());

    private static BackspaceStringCompareBenchmarks BuildHarness()
    {
        var harness = new BackspaceStringCompareBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
