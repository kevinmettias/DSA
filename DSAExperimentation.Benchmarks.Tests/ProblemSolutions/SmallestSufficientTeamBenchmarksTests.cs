using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestSufficientTeamBenchmarks (ARCHITECTURE 17.9): both arms are
// SmallestSufficientTeamSolution's searches over the same SkillMasks, so a harness whose arms
// disagree is timing two different problems. Setup derives the masks from SkillCount alone, so
// the same SkillCount must rebuild the same workload.
//
// Both arms return the team itself. Setup gives each skill exactly PeoplePerSkill dedicated
// people, listed skill by skill, so nobody covers more than one skill, no team smaller than
// SkillCount can cover them all, and a smallest team is one person per skill: person p covers
// skill p / PeoplePerSkill. Each arm's team is asserted against that size and that coverage,
// which keeps the comparison from being two arms sharing one wrong team.
public sealed partial class SmallestSufficientTeamBenchmarksTests
{
    private const int SmallestSkillCount = 6;

    // Restated from the benchmark: how many dedicated people each skill gets.
    private const int PeoplePerSkill = 3;

    // One dedicated person per skill, and no person covers more than one skill.
    private const int ExpectedTeamSize = SmallestSkillCount;

    [Fact]
    public void Setup_SameSkillCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_OnePersonPerSkill_PicksOnePersonForEverySkill() =>
        AssertOnePersonPerSkill(BuildHarness().BruteForceRecursion());

    [Fact]
    public void MemoizedRecursion_OnePersonPerSkill_PicksOnePersonForEverySkill() =>
        AssertOnePersonPerSkill(BuildHarness().MemoizedRecursion());

    private static void AssertOnePersonPerSkill(int[] team)
    {
        Assert.Equal(ExpectedTeamSize, team.Length);
        Assert.Equal(Enumerable.Range(0, SmallestSkillCount), team.Select(person => person / PeoplePerSkill).Order());
    }

    private static SmallestSufficientTeamBenchmarks BuildHarness()
    {
        var harness = new SmallestSufficientTeamBenchmarks { SkillCount = SmallestSkillCount };
        harness.Setup();

        return harness;
    }
}
