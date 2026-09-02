using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignANumberContainerSystem;

// LeetCode 2349. Design a Number Container System: each number's ever-assigned indices
// live in a min Heap<int,MinHeapOrder<int>> (Find always wants the smallest surviving
// index for that number), and a HashMap<int,int> tracks each index's *current* number so
// stale heap entries - indices later overwritten with a different number - can be
// lazily discarded from the top instead of removed mid-heap.
public sealed partial class DesignANumberContainerSystemTests
{
    [Fact]
    public void NumberContainers_LeetCodeExample_TracksSmallestIndexAcrossReplacement()
    {
        var containers = new NumberContainers();

        Assert.Equal(-1, containers.Find(10));

        containers.Change(2, 10);
        containers.Change(1, 10);
        containers.Change(3, 10);
        containers.Change(5, 10);
        Assert.Equal(1, containers.Find(10));

        containers.Change(1, 20);
        Assert.Equal(2, containers.Find(10));
    }

    [Fact]
    public void Find_NumberNeverAssigned_ReturnsNegativeOne()
    {
        var containers = new NumberContainers();

        containers.Change(0, 5);

        Assert.Equal(-1, containers.Find(99));
    }

    [Fact]
    public void Change_ReplacesIndexRepeatedly_FindReflectsOnlyCurrentAssignment()
    {
        var containers = new NumberContainers();

        containers.Change(4, 7);
        containers.Change(4, 8);
        containers.Change(4, 7);

        Assert.Equal(4, containers.Find(7));
        Assert.Equal(-1, containers.Find(8));
    }

    private sealed class NumberContainers
    {
        private readonly HashMap<int, int> _numberByIndex = new();
        private readonly HashMap<int, Heap<int, MinHeapOrder<int>>> _indicesByNumber = new();

        public void Change(int index, int number)
        {
            _numberByIndex.Set(index, number);

            if (!_indicesByNumber.TryGetValue(number, out var indices))
            {
                indices = new Heap<int, MinHeapOrder<int>>();
                _indicesByNumber.Set(number, indices);
            }

            indices.Push(index);
        }

        public int Find(int number)
        {
            if (!_indicesByNumber.TryGetValue(number, out var indices))
            {
                return -1;
            }

            while (indices.TryPeek(out var index))
            {
                if (_numberByIndex.TryGetValue(index, out var current) && current == number)
                {
                    return index;
                }

                indices.TryPop(out _);
            }

            return -1;
        }
    }
}
