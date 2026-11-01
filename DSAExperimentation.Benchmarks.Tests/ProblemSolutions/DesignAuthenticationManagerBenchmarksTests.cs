using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAuthenticationManagerBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is a count derived from the script
// rather than from either arm: the script's clock has to stay inside the time-to-live until it counts.
public sealed partial class DesignAuthenticationManagerBenchmarksTests
{
    private const int SmallestLength = 200;

    // The clock ticks once per call - Length generates, Length renews, then the count - and the
    // benchmark's time-to-live of 2000 outlasts all of them, so every generated token survives.
    private const int ExpectedSurvivorCount = SmallestLength;

    // The replay's last call comes at time 2 * Length + 1, still inside every token's time to live.
    // Half the probes name an unknown id, which renews nothing, and the other half renew a live token,
    // which keeps it live: exactly the Length generated tokens survive.
    [Fact]
    public void LinearScanList_HalfHitHalfMissRenewProbes_CountsEveryGeneratedToken() =>
        Assert.Equal(ExpectedSurvivorCount, BuildHarness().LinearScanList());

    private static DesignAuthenticationManagerBenchmarks BuildHarness()
    {
        var harness = new DesignAuthenticationManagerBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
