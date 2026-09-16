using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameIXBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the per-position maximum the LC 3660 jumps can reach - so a
// harness whose arms disagree is timing two different problems. Each arm answers with the whole
// result array, whose order is pinned by the problem (one entry per index), so the outer order is
// compared as well as the values. The workload is a seeded fixture, so the same Length must
// rebuild the same array.
public sealed partial class JumpGameIXBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameJumpArray() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().JumpBfs()),
            AnswerText.Of(BuildHarness().JumpBfs()));

    [Fact]
    public void JumpBfs_SeededArray_AgreesWithAdjacentUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.AdjacentUnionFind()), AnswerText.Of(harness.JumpBfs()));
    }

    [Fact]
    public void AdjacentUnionFind_SeededArray_AgreesWithJumpBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.JumpBfs()), AnswerText.Of(harness.AdjacentUnionFind()));
    }

    private static JumpGameIXBenchmarks BuildHarness()
    {
        var harness = new JumpGameIXBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
