using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.SmallestSufficientTeam;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestSufficientTeam;

// Harness only: both strategies are SmallestSufficientTeamSolution's - the
// unmemoized bitmask recursion and the same recursion routed through Memoizer.
// LeetCode accepts ANY minimal sufficient team, so each example pins the team's
// size and the assertion re-checks coverage with this repo's Set<string> rather
// than demanding one particular set of indices.
public sealed class SmallestSufficientTeamTests
{
    public static TheoryData<string[], string[][], int> Examples =>
        new()
        {
            { ["java", "nodejs", "reactjs"], [["java"], ["nodejs"], ["nodejs", "reactjs"]], 2 },
            {
                ["algorithms", "math", "java", "reactjs", "csharp", "aws"],
                [
                    ["algorithms", "math", "java"],
                    ["algorithms", "math", "reactjs"],
                    ["java", "csharp", "aws"],
                    ["reactjs", "csharp"],
                    ["csharp", "math"],
                    ["aws", "java"],
                ],
                2
            },
            { ["a", "b"], [["a", "b"], ["a"], ["b"]], 1 },
            { ["a"], [["a"]], 1 },
            { ["a", "b", "c"], [["a"], ["b"], ["c"]], 3 },
            { ["a", "b", "c", "d"], [["a", "b"], ["c", "d"], ["a", "c"], ["b", "d"]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestTeamByBruteForceRecursion_LeetCodeExamples_ReturnsMinimalCoveringTeam(
        string[] reqSkills, string[][] people, int expectedSize)
    {
        var team = SmallestSufficientTeamSolution.SmallestTeamByBruteForceRecursion(reqSkills, people);

        AssertIsMinimalSufficientTeam(team, reqSkills, people, expectedSize);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestTeamByMemoizedBitmask_LeetCodeExamples_ReturnsMinimalCoveringTeam(
        string[] reqSkills, string[][] people, int expectedSize)
    {
        var team = SmallestSufficientTeamSolution.SmallestTeamByMemoizedBitmask(reqSkills, people);

        AssertIsMinimalSufficientTeam(team, reqSkills, people, expectedSize);
    }

    private static void AssertIsMinimalSufficientTeam(
        int[] team, string[] reqSkills, string[][] people, int expectedSize)
    {
        Assert.Equal(expectedSize, team.Length);

        var covered = new Set<string>();
        foreach (var personIndex in team)
        {
            foreach (var skill in people[personIndex])
            {
                covered.TryAdd(skill);
            }
        }

        foreach (var skill in reqSkills)
        {
            Assert.True(covered.Has(skill));
        }
    }
}
