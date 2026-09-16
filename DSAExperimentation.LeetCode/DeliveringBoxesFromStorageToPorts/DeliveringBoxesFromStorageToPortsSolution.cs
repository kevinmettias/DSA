using WindowDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.DeliveringBoxesFromStorageToPorts;

// LeetCode 1687. Delivering Boxes from Storage to Ports: boxes must be delivered
// in order, a ship carries at most maxBoxes of them weighing at most maxWeight in
// one trip, and the answer is the fewest legs the whole delivery takes.
//
// Both strategies run the same dp: dp[i] is the cheapest delivery of the first i
// boxes, and one trip covering boxes [j, i) costs 2 + (port switches strictly
// inside it) - one leg out to the first port, one leg back from the last, plus an
// extra leg per change of destination along the way. Substituting
// BoxDeliverySchedule's prefix counts turns
// "minimize dp[j] + 2 + switches(i) - switches(j + 1)" into
// "minimize dp[j] - switches(j + 1) over a window of j", whose left edge is driven
// by two constraints at once (box count AND total weight) with a two-pointer
// sweep.
//
// The strategies differ only in how that minimum is found: rescanned from scratch
// at every position, or carried in this repo's own Deque<int> as a monotonic index
// window - the same technique ConstrainedSubsequenceSum applies to LC 1425, here
// tracking a MINIMUM instead of a maximum.
internal static class DeliveringBoxesFromStorageToPortsSolution
{
    // Every trip pays one leg out to its first port and one leg back to storage,
    // whatever it carries.
    private const int TripBaseCost = 2;

    public static int MinTripsByWindowRescan(int[][] boxes, int maxBoxes, int maxWeight)
        => MinTripsByWindowRescan(BoxDeliverySchedule.Build(boxes), maxBoxes, maxWeight);

    // The textbook arm: at every position, advance the window's left edge and then
    // rescan the whole of it for the best carry-over. Deliberately plain BCL arrays
    // and loops - it is the O(n * maxBoxes) arm the composed one below has to beat.
    public static int MinTripsByWindowRescan(BoxDeliverySchedule schedule, int maxBoxes, int maxWeight)
    {
        var boxCount = schedule.BoxCount;
        var dp = new int[boxCount + 1];
        var left = 0;

        for (var i = 1; i <= boxCount; i++)
        {
            while (i - left > maxBoxes || schedule.WeightOfFirst(i) - schedule.WeightOfFirst(left) > maxWeight)
            {
                left++;
            }

            dp[i] = TripBaseCost + schedule.PortSwitchesAmongFirst(i) + BestCarryOver(schedule, dp, left, i);
        }

        return dp[boxCount];
    }

    private static int BestCarryOver(BoxDeliverySchedule schedule, int[] dp, int left, int end)
    {
        var best = int.MaxValue;

        for (var j = left; j < end; j++)
        {
            best = Math.Min(best, dp[j] - schedule.PortSwitchesAmongFirst(j + 1));
        }

        return best;
    }

    public static int MinTripsByMonotonicDeque(int[][] boxes, int maxBoxes, int maxWeight)
        => MinTripsByMonotonicDeque(BoxDeliverySchedule.Build(boxes), maxBoxes, maxWeight);

    // The composed arm: the same dp, with the window's minimum carried in a
    // monotonic Deque<int> of candidate indices. Each index is pushed and popped at
    // most once, so the rescan collapses into a single O(n) sweep.
    public static int MinTripsByMonotonicDeque(BoxDeliverySchedule schedule, int maxBoxes, int maxWeight)
    {
        var boxCount = schedule.BoxCount;
        var dp = new int[boxCount + 1];
        var window = new TripWindow(schedule, maxBoxes, maxWeight);

        for (var i = 1; i <= boxCount; i++)
        {
            window.AdvanceAndComputeTrip(i, dp);
        }

        return dp[boxCount];
    }

    // The sliding window itself: its left edge tracks the two load constraints, and
    // its contents stay increasing in dp[j] - switches(j + 1), so the front is
    // always the best split point still reachable.
    private sealed class TripWindow(BoxDeliverySchedule schedule, int maxBoxes, int maxWeight)
    {
        private readonly WindowDeque _window = CreateSeededWindow();
        private int _left;

        // Delivering nothing costs nothing, so index 0 is always a valid split.
        private static WindowDeque CreateSeededWindow()
        {
            var window = new WindowDeque();
            window.PushBack(0);

            return window;
        }

        public void AdvanceAndComputeTrip(int deliveredCount, int[] dp)
        {
            AdvanceLeftBound(deliveredCount);
            dp[deliveredCount] = ComputeTripCost(deliveredCount, dp);
            MaintainBackWindow(deliveredCount, dp);
        }

        private void AdvanceLeftBound(int deliveredCount)
        {
            while (deliveredCount - _left > maxBoxes
                || schedule.WeightOfFirst(deliveredCount) - schedule.WeightOfFirst(_left) > maxWeight)
            {
                _left++;
            }
        }

        private int ComputeTripCost(int deliveredCount, int[] dp)
        {
            while (_window.TryPeekFront(out var frontIndex) && frontIndex < _left)
            {
                _window.TryPopFront(out _);
            }

            _window.TryPeekFront(out var bestIndex);

            return TripBaseCost + schedule.PortSwitchesAmongFirst(deliveredCount) + CarryOver(bestIndex, dp);
        }

        // The last position has no successor to split at, and its switch prefix
        // would read past the end of the schedule.
        private void MaintainBackWindow(int deliveredCount, int[] dp)
        {
            if (deliveredCount == dp.Length - 1)
            {
                return;
            }

            var candidate = CarryOver(deliveredCount, dp);

            while (_window.TryPeekBack(out var backIndex) && CarryOver(backIndex, dp) >= candidate)
            {
                _window.TryPopBack(out _);
            }

            _window.PushBack(deliveredCount);
        }

        private int CarryOver(int index, int[] dp) => dp[index] - schedule.PortSwitchesAmongFirst(index + 1);
    }
}
