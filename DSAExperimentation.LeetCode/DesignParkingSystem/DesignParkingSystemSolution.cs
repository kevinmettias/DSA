using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignParkingSystem;

// LeetCode 1603. Design Parking System: three independent slot counters, one per car
// type, each AddCar taking a slot of the matching type or refusing once that type has
// run out.
//
// This is a design problem - LeetCode's own shape is a stateful object with a
// constructor and one operation, not a single return value - so "every strategy for
// the problem" (ARCHITECTURE.md §17.3) takes the form of two full classes
// implementing the shared IParkingSystem surface below, the same shape
// DesignBrowserHistorySolution uses for its own instance-API problem (LC 1472).
//
// Both are O(1) per call; the question the pair answers is whether keying three
// counters by car type in this repo's own HashMap<int, int> costs anything measurable
// over three raw fields when the key space is, in this problem, always exactly three.
internal static class DesignParkingSystemSolution
{
    private const int BigCarType = 1;
    private const int MediumCarType = 2;
    private const int SmallCarType = 3;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without restating
    // it.
    internal interface IParkingSystem
    {
        bool AddCar(int carType);
    }

    // The textbook baseline this composition has to justify itself against: one field
    // per car type and an if/else dispatch, deliberately without any container at all.
    internal sealed class ParkingSystemByThreeFields : IParkingSystem
    {
        private int _big;
        private int _medium;
        private int _small;

        public ParkingSystemByThreeFields(int big, int medium, int small)
        {
            _big = big;
            _medium = medium;
            _small = small;
        }

        public bool AddCar(int carType)
        {
            if (carType == BigCarType)
            {
                return TryTakeSlot(ref _big);
            }

            if (carType == MediumCarType)
            {
                return TryTakeSlot(ref _medium);
            }

            return TryTakeSlot(ref _small);
        }

        private static bool TryTakeSlot(ref int remaining)
        {
            if (remaining <= 0)
            {
                return false;
            }

            remaining--;
            return true;
        }
    }

    // The composed answer: the same three counters held in this repo's own
    // HashMap<int, int> keyed by car type, so adding a car is one lookup and one
    // write rather than a dispatch chain that grows a branch per new type.
    internal sealed class ParkingSystemByHashMap : IParkingSystem
    {
        private readonly HashMap<int, int> _remaining = new();

        public ParkingSystemByHashMap(int big, int medium, int small)
        {
            _remaining.Set(BigCarType, big);
            _remaining.Set(MediumCarType, medium);
            _remaining.Set(SmallCarType, small);
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
