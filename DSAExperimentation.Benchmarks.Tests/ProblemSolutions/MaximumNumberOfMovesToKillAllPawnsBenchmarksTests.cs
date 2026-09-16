using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfMovesToKillAllPawnsBenchmarks (ARCHITECTURE 17.9): both arms
// are MaximumNumberOfMovesToKillAllPawnsSolution's competing strategies for one question - the
// exponential brute-force minimax against the same minimax over a reduced knight-distance graph -
// so a harness whose arms disagree is timing two different problems. Both answer with a single
// move count, compared directly.
//
// The prepared KnightDistances is hoisted into a private field by Setup, but both arms only read
// it, so one harness instance is safe to call twice in either order.
public sealed partial class MaximumNumberOfMovesToKillAllPawnsBenchmarksTests
{
    private const int SmallestPawnCount = 5;

    [Fact]
    public void Setup_SamePawnCount_RebuildsTheSameGameAndDistances()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The game position and the prepared distances are private, so the rebuild is pinned
        // through the move count they produce: the same PawnCount must draw the same seeded board
        // and search it identically.
        Assert.Equal(first.BruteForceMinimax(), second.BruteForceMinimax());
        Assert.Equal(first.ReduceGraphMinimax(), second.ReduceGraphMinimax());
    }

    [Fact]
    public void BruteForceMinimax_SeededPawnGame_AgreesWithReduceGraphMinimax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphMinimax(), harness.BruteForceMinimax());
    }

    [Fact]
    public void ReduceGraphMinimax_SeededPawnGame_AgreesWithBruteForceMinimax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceMinimax(), harness.ReduceGraphMinimax());
    }

    private static MaximumNumberOfMovesToKillAllPawnsBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfMovesToKillAllPawnsBenchmarks { PawnCount = SmallestPawnCount };
        harness.Setup();

        return harness;
    }
}
