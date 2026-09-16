using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for QueensThatCanAttackTheKingBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - which queens reach the king in one move - so a
// harness whose arms disagree is timing two different problems.
//
// The agreement asserted here is weaker than the class comment's wording suggests, and this is
// said plainly rather than dressed up: both arms build LeetCode's real answer (List<(Row, Col)>)
// and the harness takes .Count off it, so what agrees is the NUMBER of attacking queens, never
// which queens they are. On this seeded board the count is also very small - the queens are drawn
// at random from a BoardSize x BoardSize square, so a queen must land exactly on one of the eight
// king rays to count at all - which makes a shared count a thin witness. It still catches an arm
// that ever reports a different number, and it is what the harness actually returns.
//
// Setup draws the queen positions from one fixed seed, so the same QueensCount must rebuild the
// same board.
public sealed partial class QueensThatCanAttackTheKingBenchmarksTests
{
    private const int SmallestQueensCount = 50;

    [Fact]
    public void Setup_SameQueensCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScanRayWalk(), BuildHarness().LinearScanRayWalk());

    [Fact]
    public void LinearScanRayWalk_SeededQueenBoard_AgreesWithSetLookupRayWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanRayWalk(), harness.SetLookupRayWalk());
    }

    [Fact]
    public void SetLookupRayWalk_SeededQueenBoard_AgreesWithTheRescanArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SetLookupRayWalk(), harness.LinearScanRayWalk());
    }

    private static QueensThatCanAttackTheKingBenchmarks BuildHarness()
    {
        var harness = new QueensThatCanAttackTheKingBenchmarks { QueensCount = SmallestQueensCount };
        harness.Setup();

        return harness;
    }
}
