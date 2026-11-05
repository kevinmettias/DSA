using DSAExperimentation.LeetCode.Solutions.NumberOfStudentsUnableToEatLunch;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfStudentsUnableToEatLunch;

// Harness only: both strategies are
// NumberOfStudentsUnableToEatLunchSolution's. This file pins them to the
// boundaries LeetCode never published - the shortest inputs the constraints allow,
// a line that stalls on its very first lap, a line already in serving order, a line
// where everyone eats only after cycling, and a stall that leaves exactly one
// student. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NumberOfStudentsUnableToEatLunchSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            // Nobody wants the only kind of sandwich there is: the line never moves.
            { [1, 1, 1], [0, 0, 0], 3 },

            // The shortest inputs the constraints allow, matching and not.
            { [1], [1], 0 },
            { [0], [1], 1 },

            // Already in order: every student is served without cycling once.
            { [0, 1], [0, 1], 0 },

            // Everyone eats, but only after students cycle to the back.
            { [0, 1, 1, 0], [1, 0, 0, 1], 0 },

            // The pile stalls with a single student still waiting.
            { [0, 0, 1], [0, 1, 1], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountStudentsByListSimulation_Cases_ReturnsStudentsLeftWhenTheLineStalls(
        int[] students, int[] sandwiches, int expected)
    {
        var actual = NumberOfStudentsUnableToEatLunchSolution.CountStudentsByListSimulation(students, sandwiches);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountStudentsByQueueStackSimulation_Cases_ReturnsStudentsLeftWhenTheLineStalls(
        int[] students, int[] sandwiches, int expected)
    {
        var actual = NumberOfStudentsUnableToEatLunchSolution.CountStudentsByQueueStackSimulation(students, sandwiches);

        Assert.Equal(expected, actual);
    }
}
