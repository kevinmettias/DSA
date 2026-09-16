using DSAExperimentation.LeetCode.BoatsToSavePeople;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BoatsToSavePeople;

// Harness only: both strategies live in BoatsToSavePeopleSolution and are asserted
// against the same examples - LeetCode's three published ones, a single passenger,
// a set where every pair is exactly at the limit, and one where everyone pairs up.
public sealed partial class BoatsToSavePeopleTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2], 3, 1 },
            { [3, 2, 2, 1], 3, 3 },
            { [3, 5, 3, 4], 5, 4 },
            { [2], 3, 1 },
            { [5, 1, 4, 2], 6, 2 },
            { [1, 1, 1, 1], 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfRescueBoatsByRepeatedScan_LeetCodeExamples_ReturnsFewestBoats(
        int[] people, int limit, int expected)
    {
        var boats = BoatsToSavePeopleSolution.NumberOfRescueBoatsByRepeatedScan(people, limit);

        Assert.Equal(expected, boats);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfRescueBoatsBySortThenTwoPointer_LeetCodeExamples_ReturnsFewestBoats(
        int[] people, int limit, int expected)
    {
        var boats = BoatsToSavePeopleSolution.NumberOfRescueBoatsBySortThenTwoPointer(people, limit);

        Assert.Equal(expected, boats);
    }
}
