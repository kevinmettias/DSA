using DSAExperimentation.LeetCode.Solutions.FindAllPeopleWithSecret;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindAllPeopleWithSecret;

// Harness only. Both strategies live in FindAllPeopleWithSecretSolution - the
// repeated-relaxation baseline that used to exist only as an unasserted benchmark
// arm, and the per-timestamp KeyedDisjointSet composition - and this file pins
// both to the orderings that separate them.
public sealed partial class FindAllPeopleWithSecretSolutionTests
{
    public static TheoryData<int, (int First, int Second, int Time)[], int, int[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void FindAllPeopleByRepeatedRelaxation_Cases_ReturnsEveryoneWhoLearnsTheSecret(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson, int[] expected)
    {
        var actual = FindAllPeopleWithSecretSolution.FindAllPeopleByRepeatedRelaxation(
            peopleCount, meetings, firstPerson);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindAllPeopleByKeyedDisjointSet_Cases_ReturnsEveryoneWhoLearnsTheSecret(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson, int[] expected)
    {
        var actual = FindAllPeopleWithSecretSolution.FindAllPeopleByKeyedDisjointSet(
            peopleCount, meetings, firstPerson);
        Assert.Equal(expected, actual);
    }
}
