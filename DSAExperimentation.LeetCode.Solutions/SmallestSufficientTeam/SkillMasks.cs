namespace DSAExperimentation.LeetCode.Solutions.SmallestSufficientTeam;

// LC 1125's input reduced to the two bitmasks both strategies actually search over:
// one int per person holding the required skills that person supplies, and the mask
// with every required skill still missing. Meaningless outside this problem, so it
// lives beside the solution (ARCHITECTURE.md §17.3) rather than in the library.
//
// It is also the prepared-input shape §17.4 calls for: a benchmark builds the masks
// once in [GlobalSetup] and hands this to the measured method, and since it is not
// an IEnumerable it can never be confused with the string[][] overload.
internal sealed class SkillMasks(int[] personSkillMask, int fullMask)
{
    // personSkillMask[p] has bit i set when person p has reqSkills[i].
    public int[] PersonSkillMask { get; } = personSkillMask;

    // Every required-skill bit set: the state the search starts from.
    public int FullMask { get; } = fullMask;

    public static SkillMasks Build(string[] reqSkills, string[][] people)
    {
        var skillBit = BuildSkillBitIndex(reqSkills);

        return new SkillMasks(BuildPersonSkillMasks(people, skillBit), (1 << reqSkills.Length) - 1);
    }

    // reqSkills.Length <= 16, so each required skill gets its own bit position. A BCL
    // Dictionary rather than this repo's HashMap: the baseline builds its masks here
    // too, and a baseline uses no library implementation (ARCHITECTURE 17.5).
    private static Dictionary<string, int> BuildSkillBitIndex(string[] reqSkills)
    {
        var skillBit = new Dictionary<string, int>();

        for (var i = 0; i < reqSkills.Length; i++)
        {
            skillBit[reqSkills[i]] = i;
        }

        return skillBit;
    }

    // Skills a person has that nobody requires simply never appear in the index, so
    // they contribute no bits.
    private static int[] BuildPersonSkillMasks(string[][] people, Dictionary<string, int> skillBit)
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
}
