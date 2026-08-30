using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignParkingSystem;

// LeetCode 1603. Design Parking System: three remaining-slot counters keyed by car
// type, held in this repo's own HashMap<int,int> (TwoSumTests precedent) rather than
// three separate fields - AddCar decrements the matching slot and refuses once it
// has already hit zero.
public sealed partial class DesignParkingSystemTests
{
    [Fact]
    public void AddCar_LeetCodeExample_MatchesExpectedSequence()
    {
        var system = new ParkingSystem(1, 1, 0);

        Assert.True(system.AddCar(1));
        Assert.True(system.AddCar(2));
        Assert.False(system.AddCar(3));
        Assert.False(system.AddCar(1));
    }

    [Fact]
    public void AddCar_ZeroCapacityForEveryType_AlwaysRefuses()
    {
        var system = new ParkingSystem(0, 0, 0);

        Assert.False(system.AddCar(1));
        Assert.False(system.AddCar(2));
        Assert.False(system.AddCar(3));
    }

    private sealed class ParkingSystem
    {
        private readonly HashMap<int, int> _remaining = new();

        public ParkingSystem(int big, int medium, int small)
        {
            _remaining.Set(1, big);
            _remaining.Set(2, medium);
            _remaining.Set(3, small);
        }

        public bool AddCar(int carType)
        {
            _remaining.TryGetValue(carType, out var slots);

            if (slots <= 0)
            {
                return false;
            }

            _remaining.Set(carType, slots - 1);
            return true;
        }
    }
}
