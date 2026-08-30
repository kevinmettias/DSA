using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeliveringBoxesFromStorageToPorts;

// LeetCode 1687. Delivering Boxes from Storage to Ports: dp[i] = minimum trips to
// deliver the first i boxes. A single trip covering boxes[j..i-1] costs
// 2 + (port switches strictly inside that range) - one leg out to the first port,
// one leg back from the last, plus one extra leg per distinct-consecutive-port
// change along the way. Substituting a prefix-summed switch count turns
// "minimize dp[j] + 2 + switchPrefix[i] - switchPrefix[j+1] over valid j" into
// "minimize dp[j] - switchPrefix[j+1] across a sliding window of j" - the same
// monotonic-deque-over-a-dp-array technique ConstrainedSubsequenceSumTests
// already uses, except the window's left edge is driven by two constraints
// (maxBoxes count AND maxWeight sum) advanced with a two-pointer sweep instead
// of one fixed k, and the deque tracks a running MINIMUM instead of a maximum.
public sealed class DeliveringBoxesFromStorageToPortsTests
{
    [Fact]
    public void MinTrips_LeetCodeExampleOne_ReturnsFour()
    {
        int[][] boxes = [[1, 1], [2, 1], [1, 1]];

        var result = MinTrips(boxes, maxBoxes: 3, maxWeight: 3);

        Assert.Equal(4, result);
    }

    [Fact]
    public void MinTrips_LeetCodeExampleTwo_ReturnsSix()
    {
        int[][] boxes = [[1, 2], [3, 3], [3, 1], [3, 1], [2, 4]];

        var result = MinTrips(boxes, maxBoxes: 3, maxWeight: 6);

        Assert.Equal(6, result);
    }

    [Fact]
    public void MinTrips_AllSamePortFitsInOneTrip_ReturnsSingleTripCostWithNoSwitches()
    {
        int[][] boxes = [[1, 1], [1, 1], [1, 1]];

        var result = MinTrips(boxes, maxBoxes: 3, maxWeight: 3);

        Assert.Equal(2, result);
    }

    [Fact]
    public void MinTrips_MaxBoxesOfOneForcesOneBoxPerTrip_ReturnsSumOfIndividualTripCosts()
    {
        int[][] boxes = [[1, 1], [1, 1]];

        var result = MinTrips(boxes, maxBoxes: 1, maxWeight: 10);

        Assert.Equal(4, result);
    }

    private static int MinTrips(int[][] boxes, int maxBoxes, int maxWeight)
    {
        var n = boxes.Length;
        var switchPrefix = new int[n + 1];
        var weightPrefix = new long[n + 1];

        for (var i = 1; i <= n; i++)
        {
            weightPrefix[i] = weightPrefix[i - 1] + boxes[i - 1][1];
            switchPrefix[i] = i < 2
                ? 0
                : switchPrefix[i - 1] + (boxes[i - 2][0] != boxes[i - 1][0] ? 1 : 0);
        }

        var dp = new int[n + 1];
        var window = new RepoDeque();
        window.PushBack(0);
        var left = 0;

        for (var i = 1; i <= n; i++)
        {
            while (i - left > maxBoxes || weightPrefix[i] - weightPrefix[left] > maxWeight)
            {
                left++;
            }

            while (window.TryPeekFront(out var frontIndex) && frontIndex < left)
            {
                window.TryPopFront(out _);
            }

            window.TryPeekFront(out var bestIndex);
            dp[i] = 2 + switchPrefix[i] + (dp[bestIndex] - switchPrefix[bestIndex + 1]);

            if (i == n)
            {
                continue;
            }

            var candidate = dp[i] - switchPrefix[i + 1];
            while (window.TryPeekBack(out var backIndex) && dp[backIndex] - switchPrefix[backIndex + 1] >= candidate)
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return dp[n];
    }
}
