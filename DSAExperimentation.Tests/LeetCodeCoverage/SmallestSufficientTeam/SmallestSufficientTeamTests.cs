using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestSufficientTeam;

// LeetCode 1125. Smallest Sufficient Team: bitmask DP over "which required skills are
// still missing" via this repo's own Memoizer, the same "int bitmask memo state" shape
// CanIWinTests.cs already establishes - reqSkills.Length <= 16 fits the memo key, and
// people.Length <= 60 fits a long bitmask of CHOSEN PEOPLE, so the memoized result IS
// the team itself, not just its size. Each recursive step covers only the lowest still-
// missing skill bit (every sufficient team must include SOME person with that skill),
// trying each such person and keeping whichever completion has the fewest people.
// HashMap<string,int> assigns each skill name its bit position; Set<string> (both
// composed rather than reimplemented) verifies coverage in the assertions.
public sealed class SmallestSufficientTeamTests
{
    [Fact]
    public void SmallestSufficientTeam_ThreePeopleThreeSkills_ReturnsMinimalCoveringTeam()
    {
        string[] reqSkills = ["java", "nodejs", "reactjs"];
        string[][] people = [["java"], ["nodejs"], ["nodejs", "reactjs"]];

        var team = SmallestSufficientTeam(reqSkills, people);

        AssertIsMinimalSufficientTeam(team, reqSkills, people, expectedSize: 2);
    }

    [Fact]
    public void SmallestSufficientTeam_SixPeopleSixSkills_ReturnsMinimalCoveringTeam()
    {
        string[] reqSkills = ["algorithms", "math", "java", "reactjs", "csharp", "aws"];
        string[][] people =
        [
            ["algorithms", "math", "java"],
            ["algorithms", "math", "reactjs"],
            ["java", "csharp", "aws"],
            ["reactjs", "csharp"],
            ["csharp", "math"],
            ["aws", "java"],
        ];

        var team = SmallestSufficientTeam(reqSkills, people);

        AssertIsMinimalSufficientTeam(team, reqSkills, people, expectedSize: 2);
    }

    [Fact]
    public void SmallestSufficientTeam_OnePersonAlreadyCoversEverything_ReturnsSinglePerson()
    {
        string[] reqSkills = ["a", "b"];
        string[][] people = [["a", "b"], ["a"], ["b"]];

        var team = SmallestSufficientTeam(reqSkills, people);

        AssertIsMinimalSufficientTeam(team, reqSkills, people, expectedSize: 1);
        Assert.Equal(0, team[0]);
    }

    private static void AssertIsMinimalSufficientTeam(int[] team, string[] reqSkills, string[][] people, int expectedSize)
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

    private static int[] SmallestSufficientTeam(string[] reqSkills, string[][] people)
    {
        var skillBit = BuildSkillBitIndex(reqSkills);
        var personSkillMask = BuildPersonSkillMasks(people, skillBit);
        var fullMask = (1 << reqSkills.Length) - 1;
        var teamMask = ComputeTeamMask(personSkillMask, fullMask);

        return ExtractTeam(teamMask, people.Length);
    }

    private static HashMap<string, int> BuildSkillBitIndex(string[] reqSkills)
    {
        var skillBit = new HashMap<string, int>();
        for (var i = 0; i < reqSkills.Length; i++)
        {
            skillBit.Set(reqSkills[i], i);
        }

        return skillBit;
    }

    private static int[] BuildPersonSkillMasks(string[][] people, HashMap<string, int> skillBit)
    {
        var personSkillMask = new int[people.Length];
        for (var p = 0; p < people.Length; p++)
        {
            foreach (var skill in people[p])
            {
                if (skillBit.TryGetValue(skill, out var bit))
                {
                    personSkillMask[p] |= 1 << bit;
                }
            }
        }

        return personSkillMask;
    }

    private static long ComputeTeamMask(int[] personSkillMask, int fullMask)
        => Memoizer.Memoize<int, long>(fullMask, (missing, smallestTeamFor) => Best(personSkillMask, missing, smallestTeamFor));

    private static long Best(int[] personSkillMask, int missing, Func<int, long> smallestTeamFor)
    {
        if (missing == 0)
        {
            return 0L;
        }

        return FindBestCandidate(personSkillMask, missing, smallestTeamFor);
    }

    private static long FindBestCandidate(int[] personSkillMask, int missing, Func<int, long> smallestTeamFor)
    {
        var targetBit = missing & -missing;
        var best = -1L;

        for (var p = 0; p < personSkillMask.Length; p++)
        {
            if ((personSkillMask[p] & targetBit) == 0)
            {
                continue;
            }

            var candidate = smallestTeamFor(missing & ~personSkillMask[p]) | (1L << p);
            if (best == -1 || PopCount(candidate) < PopCount(best))
            {
                best = candidate;
            }
        }

        return best;
    }

    private static int[] ExtractTeam(long teamMask, int peopleCount)
    {
        var team = new List<int>();
        for (var p = 0; p < peopleCount; p++)
        {
            if ((teamMask & (1L << p)) != 0)
            {
                team.Add(p);
            }
        }

        return [.. team];
    }

    private static int PopCount(long mask)
    {
        var count = 0;
        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }
}
