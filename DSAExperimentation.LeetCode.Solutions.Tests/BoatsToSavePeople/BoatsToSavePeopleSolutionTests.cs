using DSAExperimentation.LeetCode.Solutions.BoatsToSavePeople;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BoatsToSavePeople;

// Harness only: both strategies live in BoatsToSavePeopleSolution and are asserted
// against the same cases - LeetCode's three published ones, a single passenger,
// a set where every pair is exactly at the limit, and one where everyone pairs up.
public sealed partial class BoatsToSavePeopleSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [2], 3, 1 },
            { [5, 1, 4, 2], 6, 2 },
            { [1, 1, 1, 1], 2, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfRescueBoatsByRepeatedScan_Cases_ReturnsFewestBoats(
        int[] people, int limit, int expected)
    {
        var boats = BoatsToSavePeopleSolution.NumberOfRescueBoatsByRepeatedScan(people, limit);

        Assert.Equal(expected, boats);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void NumberOfRescueBoatsBySortThenTwoPointer_Cases_ReturnsFewestBoats(
        int[] people, int limit, int expected)
    {
        var boats = BoatsToSavePeopleSolution.NumberOfRescueBoatsBySortThenTwoPointer(people, limit);

        Assert.Equal(expected, boats);
    }
}
