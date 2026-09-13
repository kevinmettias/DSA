using static DSAExperimentation.LeetCode.DesignParkingSystem.DesignParkingSystemSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignParkingSystem;

// Harness only: both strategies live in DesignParkingSystemSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes the
// three capacities plus a script of requested car types and the accept/refuse answer
// for each - the same call-script shape DesignBrowserHistoryTests uses for its own
// instance-API problem.
public sealed class DesignParkingSystemTests
{
    public static TheoryData<int, int, int, int[], bool[]> Examples =>
        new()
        {
            // LeetCode's published example: one big slot, one medium, no small.
            { 1, 1, 0, [1, 2, 3, 1], [true, true, false, false] },

            // Nothing is available for any type, so every request is refused.
            { 0, 0, 0, [1, 2, 3], [false, false, false] },

            // Each type exhausts independently: running out of big says nothing about
            // medium or small.
            { 1, 2, 3, [1, 1, 2, 2, 2, 3], [true, false, true, true, false, true] },

            // A refused request must not consume a slot, so the type still has room on
            // the next call - here small is asked for after big has run dry.
            { 2, 0, 1, [1, 1, 1, 2, 3, 3], [true, true, false, false, true, false] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParkingSystemByThreeFields_LeetCodeExamples_MatchesExpectedSequence(
        int big, int medium, int small, int[] carTypes, bool[] expected) =>
        RunScript(new ParkingSystemByThreeFields(big, medium, small), carTypes, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParkingSystemByHashMap_LeetCodeExamples_MatchesExpectedSequence(
        int big, int medium, int small, int[] carTypes, bool[] expected) =>
        RunScript(new ParkingSystemByHashMap(big, medium, small), carTypes, expected);

    private static void RunScript(IParkingSystem system, int[] carTypes, bool[] expected)
    {
        for (var i = 0; i < carTypes.Length; i++)
        {
            Assert.Equal(expected[i], system.AddCar(carTypes[i]));
        }
    }
}
