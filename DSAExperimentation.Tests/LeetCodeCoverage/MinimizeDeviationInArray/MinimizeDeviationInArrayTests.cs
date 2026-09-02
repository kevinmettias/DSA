using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeDeviationInArray;

// LeetCode 1675. Minimize Deviation in Array: every odd value's only reachable
// even form is itself doubled once, so normalize every element to that "doubled if
// odd" starting point, then repeatedly halve the current largest value (tracking
// the running minimum) until the largest is odd and can't be reduced further -
// using this repo's own Heap<int,MaxHeapOrder<int>> to always offer up the current
// largest value in O(log n), the same "repeatedly reduce the current max" shape
// LastStoneWeightTests already uses for a different reduction rule.
public sealed partial class MinimizeDeviationInArrayTests
{
    [Fact]
    public void MinimumDeviation_ClassicExample_ReturnsOne()
    {
        int[] nums = [1, 2, 3, 4];

        Assert.Equal(1, MinimumDeviation(nums));
    }

    [Fact]
    public void MinimumDeviation_AllEvenValues_ReturnsThree()
    {
        int[] nums = [2, 10, 8];

        Assert.Equal(3, MinimumDeviation(nums));
    }

    [Fact]
    public void MinimumDeviation_SingleElement_ReturnsZero()
    {
        int[] nums = [5];

        Assert.Equal(0, MinimumDeviation(nums));
    }

    private static int MinimumDeviation(int[] nums)
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();
        var min = int.MaxValue;

        foreach (var num in nums)
        {
            var value = num % 2 == 1 ? num * 2 : num;
            heap.Push(value);
            min = Math.Min(min, value);
        }

        var reducer = new DeviationReducer(heap, min);
        return reducer.Reduce();
    }

    private sealed class DeviationReducer(Heap<int, MaxHeapOrder<int>> heap, int min)
    {
        public int Reduce()
        {
            var deviation = int.MaxValue;

            while (true)
            {
                heap.TryPop(out var max);
                deviation = Math.Min(deviation, max - min);

                if (max % 2 != 0)
                {
                    break;
                }

                var half = max / 2;
                min = Math.Min(min, half);
                heap.Push(half);
            }

            return deviation;
        }
    }
}
