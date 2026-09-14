using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignMemoryAllocator;

// LeetCode 2502. Design Memory Allocator: a memory array of n units, all free.
// allocate(size, mID) stamps mID into the leftmost run of `size` consecutive free
// units and reports that run's start index, or -1 when no such run exists;
// free(mID) releases every unit currently carrying mID and reports how many units
// that was.
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (§17.3) takes the form of two classes implementing the shared
// IMemoryAllocatorStrategy surface below rather than two static methods sharing an
// <Operation>By<Strategy> name - the same shape DesignANumberContainerSystemSolution
// and DesignTaskManagerSolution use, and for the same reason: a Design problem is a
// sequence of mutating calls against one instance, so there is no prepared input to
// hoist into a benchmark's [GlobalSetup] the way §17.4 hoists OpenTheLock's built
// graph. Each [Benchmark] arm constructs its own instance and replays the same call
// script instead.
//
// Both strategies share the identical Allocate - a leftmost-free-run scan over a
// plain int[] memory array, which every accepted solution to this problem does, so
// it is not the axis being compared. They differ only in Free.
internal static class DesignMemoryAllocatorSolution
{
    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it.
    internal interface IMemoryAllocatorStrategy
    {
        int Allocate(int size, int mID);

        int Free(int mID);
    }

    // The textbook answer: nothing but the memory array itself. Free rescans the
    // whole array for every unit still stamped with mID, on every call - it has to,
    // since a correct Free cannot stop early without first ruling out a later
    // match. Deliberately without this repo's primitives (§17.5); it is the arm the
    // HashMap-indexed strategy below has to justify itself against.
    internal sealed class MemoryAllocatorByArrayScan(int n) : IMemoryAllocatorStrategy
    {
        private readonly int[] _memory = new int[n];

        public int Allocate(int size, int mID)
        {
            var runStart = FindLeftmostFreeRun(_memory, size);

            if (runStart < 0)
            {
                return LeetCodeAnswer.None;
            }

            for (var i = runStart; i < runStart + size; i++)
            {
                _memory[i] = mID;
            }

            return runStart;
        }

        public int Free(int mID)
        {
            var freed = 0;

            for (var i = 0; i < _memory.Length; i++)
            {
                if (_memory[i] == mID)
                {
                    _memory[i] = FreeUnit;
                    freed++;
                }
            }

            return freed;
        }
    }

    // This repo's own primitives: a HashMap<mID, DynamicArray<index>> remembers
    // exactly which units each mID currently owns, so Free visits those indices and
    // nothing else instead of walking the whole array - the same "HashMap index over
    // an array" shape DesignANumberContainerSystemSolution uses, just tracking a set
    // of indices per key instead of one. Allocate appends to the entry rather than
    // replacing it, because LeetCode lets the same mID own several disjoint blocks
    // and a single free(mID) has to release all of them.
    internal sealed class MemoryAllocatorByHashMapIndex(int n) : IMemoryAllocatorStrategy
    {
        private readonly int[] _memory = new int[n];
        private readonly HashMap<int, DynamicArray<int>> _unitsByMemoryId = new();

        public int Allocate(int size, int mID)
        {
            var runStart = FindLeftmostFreeRun(_memory, size);

            if (runStart < 0)
            {
                return LeetCodeAnswer.None;
            }

            var units = UnitsFor(mID);

            for (var i = runStart; i < runStart + size; i++)
            {
                _memory[i] = mID;
                units.Add(i);
            }

            return runStart;
        }

        private DynamicArray<int> UnitsFor(int mID)
        {
            if (!_unitsByMemoryId.TryGetValue(mID, out var units))
            {
                units = new DynamicArray<int>();
                _unitsByMemoryId.Set(mID, units);
            }

            return units;
        }

        public int Free(int mID)
        {
            if (!_unitsByMemoryId.TryGetValue(mID, out var units))
            {
                return 0;
            }

            for (var i = 0; i < units.Count; i++)
            {
                _memory[units.Get(i)] = FreeUnit;
            }

            _unitsByMemoryId.TryRemove(mID);
            return units.Count;
        }
    }

    // 0 is LeetCode's "this unit belongs to nobody"; mIDs are >= 1.
    private const int FreeUnit = 0;

    // Shared by both strategies because it is not what they differ on: the leftmost
    // window of `size` consecutive free units, or -1.
    private static int FindLeftmostFreeRun(int[] memory, int size)
    {
        var runStart = LeetCodeAnswer.None;
        var runLength = 0;

        for (var i = 0; i < memory.Length; i++)
        {
            if (memory[i] != FreeUnit)
            {
                runStart = LeetCodeAnswer.None;
                runLength = 0;
                continue;
            }

            runStart = runLength == 0 ? i : runStart;
            runLength++;

            if (runLength == size)
            {
                return runStart;
            }
        }

        return LeetCodeAnswer.None;
    }
}
