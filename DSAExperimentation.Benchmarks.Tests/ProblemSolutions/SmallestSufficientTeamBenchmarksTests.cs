using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestSufficientTeamBenchmarks (ARCHITECTURE 17.9): both arms are
// SmallestSufficientTeamSolution's searches over the same SkillMasks, so a harness whose arms
// disagree is timing two different problems. Setup derives the masks from SkillCount alone, so
// the same SkillCount must rebuild the same workload.
//
// Both arms report the team's length rather than the team, which is decisive here rather than a
// weakened proxy: Setup gives each skill exactly PeoplePerSkill dedicated people, so nobody
// covers more than one skill, no team smaller than SkillCount can cover them all, and one person
// per skill does. Asserting that size alongside the agreement keeps the comparison from being
// two arms sharing one wrong number.
public sealed partial class SmallestSufficientTeamBenchmarksTests
{
    private const int SmallestSkillCount = 6;

    // One dedicated person per skill, and no person covers more than one skill.
    private const int ExpectedTeamSize = SmallestSkillCount;

    [Fact]
    public void Setup_SameSkillCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_OnePersonPerSkill_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTeamSize, harness.BruteForceRecursion());
        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    [Fact]
    public void MemoizedRecursion_OnePersonPerSkill_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTeamSize, harness.MemoizedRecursion());
        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    private static SmallestSufficientTeamBenchmarks BuildHarness()
    {
        var harness = new SmallestSufficientTeamBenchmarks { SkillCount = SmallestSkillCount };
        harness.Setup();

        return harness;
    }
}
