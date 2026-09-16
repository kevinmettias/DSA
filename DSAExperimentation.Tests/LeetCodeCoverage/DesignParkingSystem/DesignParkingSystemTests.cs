using IParkingSystem = DSAExperimentation.LeetCode.DesignParkingSystem.DesignParkingSystemSolution.IParkingSystem;
using ParkingSystemByThreeFields = DSAExperimentation.LeetCode.DesignParkingSystem.DesignParkingSystemSolution.ParkingSystemByThreeFields;
using ParkingSystemByHashMap = DSAExperimentation.LeetCode.DesignParkingSystem.DesignParkingSystemSolution.ParkingSystemByHashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignParkingSystem;

// Harness only: both strategies live in DesignParkingSystemSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes the
// three capacities plus a script of requested car types and the accept/refuse answer
// for each - the same call-script shape DesignBrowserHistoryTests uses for its own
// instance-API problem.
public sealed partial class DesignParkingSystemTests
{
    public static TheoryData<SlotCapacities, int[], bool[]> Examples =>
        new()
        {
            // LeetCode's published example: one big slot, one medium, no small.
            { new SlotCapacities(Big: 1, Medium: 1, Small: 0), [1, 2, 3, 1], [true, true, false, false] },

            // Nothing is available for any type, so every request is refused.
            { new SlotCapacities(Big: 0, Medium: 0, Small: 0), [1, 2, 3], [false, false, false] },

            // Each type exhausts independently: running out of big says nothing about
            // medium or small.
            { new SlotCapacities(Big: 1, Medium: 2, Small: 3), [1, 1, 2, 2, 2, 3], [true, false, true, true, false, true] },

            // A refused request must not consume a slot, so the type still has room on
            // the next call - here small is asked for after big has run dry.
            { new SlotCapacities(Big: 2, Medium: 0, Small: 1), [1, 1, 1, 2, 3, 3], [true, true, false, false, true, false] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParkingSystemByThreeFields_LeetCodeExamples_MatchesExpectedSequence(
        SlotCapacities capacities, int[] carTypes, bool[] expected)
    {
        var system = new ParkingSystemByThreeFields(capacities.Big, capacities.Medium, capacities.Small);

        Assert.Equal(expected, RunScript(system, carTypes));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParkingSystemByHashMap_LeetCodeExamples_MatchesExpectedSequence(
        SlotCapacities capacities, int[] carTypes, bool[] expected)
    {
        var system = new ParkingSystemByHashMap(capacities.Big, capacities.Medium, capacities.Small);

        Assert.Equal(expected, RunScript(system, carTypes));
    }

    private static bool[] RunScript(IParkingSystem system, int[] carTypes) =>
        [.. carTypes.Select(carType => system.AddCar(carType))];

    // The three slot counts a parking system is constructed with. They travel together at
    // every call site and LeetCode's own constructor takes them as one turn, so they are
    // one thing with a name rather than three adjacent ints a caller can transpose.
    public readonly record struct SlotCapacities(int Big, int Medium, int Small);
}
