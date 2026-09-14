using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.DesignANumberContainerSystem;

// LeetCode 2349. Design a Number Container System: change(index, number) assigns a
// number to an index, find(number) reports the smallest index currently holding it.
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (§17.3) takes the form of two classes implementing the shared
// INumberContainerStrategy surface below rather than two static methods sharing an
// <Operation>By<Strategy> name - the same shape DesignTaskManagerSolution uses, and
// for the same reason: a Design problem is a sequence of mutating calls against one
// instance, so there is no prepared input to hoist into a benchmark's
// [GlobalSetup]. Each [Benchmark] arm constructs its own instance and replays the
// same call script instead.
internal static class DesignANumberContainerSystemSolution
{
    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it.
    internal interface INumberContainerStrategy
    {
        void Change(int index, int number);

        int Find(int number);
    }

    // The textbook answer: one BCL Dictionary<index, number> and a full scan of it
    // for the smallest index holding the queried number on every Find - the arm the
    // lazy-deletion heap below has to justify itself against. Deliberately without
    // this repo's primitives (§17.5).
    internal sealed class NumberContainersByLinearScan : INumberContainerStrategy
    {
        private readonly Dictionary<int, int> _numberByIndex = new();

        public void Change(int index, int number) => _numberByIndex[index] = number;

        public int Find(int number)
        {
            int? smallest = null;

            foreach (var (index, assigned) in _numberByIndex)
            {
                if (assigned != number)
                {
                    continue;
                }

                if (smallest is null || index < smallest)
                {
                    smallest = index;
                }
            }

            return smallest ?? LeetCodeAnswer.None;
        }
    }

    // This repo's own primitives: every index ever assigned to a number is pushed
    // onto that number's min Heap<int, MinHeapOrder<int>> (Find always wants the
    // smallest surviving one), and a HashMap<index, number> holds each index's
    // current authoritative assignment. Change never removes the index from the
    // heap it was previously filed under - Heap has no arbitrary remove (see
    // Heap.cs) - so Find lazily discards entries from the top whose index no longer
    // maps back to the queried number, the standard lazy-deletion trick for a
    // priority queue without one (the same shape DesignTaskManager's ExecTop and
    // DesignAFoodRatingSystem's HighestRated use).
    internal sealed class NumberContainersByLazyDeletionHeap : INumberContainerStrategy
    {
        private readonly HashMap<int, int> _numberByIndex = new();
        private readonly HashMap<int, Heap<int, MinHeapOrder<int>>> _indicesByNumber = new();

        public void Change(int index, int number)
        {
            _numberByIndex.Set(index, number);
            IndicesFor(number).Push(index);
        }

        private Heap<int, MinHeapOrder<int>> IndicesFor(int number)
        {
            if (!_indicesByNumber.TryGetValue(number, out var indices))
            {
                indices = new Heap<int, MinHeapOrder<int>>();
                _indicesByNumber.Set(number, indices);
            }

            return indices;
        }

        public int Find(int number)
        {
            if (!_indicesByNumber.TryGetValue(number, out var indices))
            {
                return LeetCodeAnswer.None;
            }

            while (indices.TryPeek(out var index))
            {
                if (_numberByIndex.TryGetValue(index, out var current) && current == number)
                {
                    return index;
                }

                indices.TryPop(out _);
            }

            return LeetCodeAnswer.None;
        }
    }
}
