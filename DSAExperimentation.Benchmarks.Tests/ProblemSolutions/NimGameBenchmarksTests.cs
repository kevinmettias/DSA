using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NimGameBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - the memoized game-theory recursion against the closed-form n % 4 != 0
// formula that recursion reduces to - so a harness whose arms disagree is answering two different
// game positions. The class carries no [GlobalSetup]: Stones is the whole workload, passed straight
// through to both arms, so the same Stones must answer both.
//
// The agreement here is WEAK BY CONSTRUCTION and the weakness is in the fixture, not the assertion:
// both declared [Params] values (20 and 1000) are multiples of four, so both arms return false for
// every harness this class can build and a constant-false stub would satisfy them. The tests are
// still load-bearing - a negation of either arm turns them red (see the mutation proof) - but they
// witness "both arms agree on a losing position", not the discriminating case. Which Stones values
// the class measures is a harness decision, so it is reported rather than changed here.
public sealed partial class NimGameBenchmarksTests
{
    private const int SmallestStones = 20;

    [Fact]
    public void CanWinByMemoizedRecursion_AgreesWithCanWinByModuloFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByModuloFormula(), harness.CanWinByMemoizedRecursion());
    }

    [Fact]
    public void CanWinByModuloFormula_AgreesWithCanWinByMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByMemoizedRecursion(), harness.CanWinByModuloFormula());
    }

    private static NimGameBenchmarks BuildHarness() => new() { Stones = SmallestStones };
}
