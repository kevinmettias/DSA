using DSAExperimentation.LeetCode.FindAllPeopleWithSecret;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllPeopleWithSecret;

// Harness only. Both strategies live in FindAllPeopleWithSecretSolution - the
// repeated-relaxation baseline that used to exist only as an unasserted benchmark
// arm, and the per-timestamp KeyedDisjointSet composition - and this file pins
// both to LeetCode's published examples plus the orderings that separate them.
public sealed partial class FindAllPeopleWithSecretTests
{
    public static TheoryData<int, (int First, int Second, int Time)[], int, int[]> Examples =>
        new()
        {
            // LC example 1: the secret walks forward through ascending timestamps.
            { 6, [(1, 2, 5), (2, 3, 8), (1, 5, 10)], 1, [0, 1, 2, 3, 5] },

            // LC example 2: person 2 meets person 1 at time 2, BEFORE person 1
            // learns anything at time 3, so a later timestamp cannot reach back.
            { 4, [(3, 1, 3), (1, 2, 2), (0, 3, 3)], 3, [0, 1, 3] },

            // LC example 3: a same-timestamp chain resolves transitively.
            { 5, [(3, 4, 2), (1, 2, 1), (2, 3, 1)], 1, [0, 1, 2, 3, 4] },

            // A same-timestamp chain stated in reverse edge order - the shape the
            // benchmark measures, where one forward pass extends the informed
            // frontier by a single hop and relaxation must keep rescanning.
            { 6, [(4, 5, 1), (3, 4, 1), (2, 3, 1), (1, 2, 1)], 1, [0, 1, 2, 3, 4, 5] },

            // No meetings at all: only the two people who started out informed.
            { 3, [], 2, [0, 2] },

            // A meeting between two uninformed people spreads nothing.
            { 4, [(1, 2, 1)], 3, [0, 3] },

            // firstPerson is person 0's own meeting partner later on, and the
            // unreached component stays unreached across every timestamp.
            { 7, [(0, 1, 1), (2, 3, 2), (4, 5, 3), (3, 4, 4)], 6, [0, 1, 6] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllPeopleByRepeatedRelaxation_LeetCodeExamples_ReturnsEveryoneWhoLearnsTheSecret(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson, int[] expected)
    {
        var actual = FindAllPeopleWithSecretSolution.FindAllPeopleByRepeatedRelaxation(
            peopleCount, meetings, firstPerson);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllPeopleByKeyedDisjointSet_LeetCodeExamples_ReturnsEveryoneWhoLearnsTheSecret(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson, int[] expected)
    {
        var actual = FindAllPeopleWithSecretSolution.FindAllPeopleByKeyedDisjointSet(
            peopleCount, meetings, firstPerson);
        Assert.Equal(expected, actual);
    }
}
