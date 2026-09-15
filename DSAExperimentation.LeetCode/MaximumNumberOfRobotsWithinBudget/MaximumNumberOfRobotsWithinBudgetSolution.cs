using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.MaximumNumberOfRobotsWithinBudget;

// LeetCode 2398. Maximum Number of Robots Within Budget: the longest contiguous
// run of robots whose cost - max(chargeTimes in the run) + runLength *
// sum(runningCosts in the run) - stays within budget.
//
// MaximumRobotsByRescanEveryLeftEdge is the textbook answer: fix a left edge and
// extend right, re-deriving the window max and sum as it goes. Because every
// runningCost is positive, cost is strictly increasing in window size for a fixed
// left edge, so the scan can stop the moment it goes over budget - still O(n) per
// left edge in the worst case, O(n^2) overall.
//
// MaximumRobotsByMonotonicDeque keeps one two-pointer window instead, with this
// repo's own Deque<int> as a monotonic decreasing-chargeTimes index window
// (SlidingWindowMaximumSolution's precedent): the front index is always the
// window's max, and the runningCosts total is maintained incrementally as the left
// edge advances rather than resummed. That same positivity is what makes shrinking
// from the left safe instead of re-scanning, so every index is pushed and popped
// at most once and the whole sweep is O(n).
internal static class MaximumNumberOfRobotsWithinBudgetSolution
{
    // Deliberately written without this repo's primitives - it is the arm the
    // sliding window below has to justify itself against.
    public static int MaximumRobotsByRescanEveryLeftEdge(int[] chargeTimes, int[] runningCosts, long budget)
    {
        var best = 0;

        for (var left = 0; left < chargeTimes.Length; left++)
        {
            var window = LongestBudgetedWindowFrom(chargeTimes, runningCosts, budget, left);
            best = Math.Max(best, window);
        }

        return best;
    }

    private static int LongestBudgetedWindowFrom(int[] chargeTimes, int[] runningCosts, long budget, int left)
    {
        var maxCharge = 0;
        long runningCostSum = 0;

        for (var right = left; right < chargeTimes.Length; right++)
        {
            maxCharge = Math.Max(maxCharge, chargeTimes[right]);
            runningCostSum += runningCosts[right];

            if (maxCharge + ((long)(right - left + 1) * runningCostSum) > budget)
            {
                return right - left;
            }
        }

        return chargeTimes.Length - left;
    }

    public static int MaximumRobotsByMonotonicDeque(int[] chargeTimes, int[] runningCosts, long budget)
    {
        var window = new BudgetWindow(chargeTimes, runningCosts, budget, new RepoDeque(), 0, 0);
        var best = 0;

        for (var right = 0; right < window.ChargeTimes.Length; right++)
        {
            window = ExtendWindow(window, right);
            best = Math.Max(best, right - window.Left + 1);
        }

        return best;
    }

    // Absorbs the robot at `right`, then advances the window's left edge until its
    // cost - the max charge time in it plus its length times its summed running cost
    // - fits the budget again, dropping each departing index's cost as it goes.
    private static BudgetWindow ExtendWindow(BudgetWindow window, int right)
    {
        PushMaxCandidate(window.ChargeTimes, window.MaxCandidates, right);
        var runningCostSum = window.RunningCostSum + window.RunningCosts[right];
        var left = window.Left;

        while (window.MaxCandidates.TryPeekFront(out var maxIndex)
            && window.ChargeTimes[maxIndex] + ((long)(right - left + 1) * runningCostSum) > window.Budget)
        {
            if (maxIndex == left)
            {
                window.MaxCandidates.TryPopFront(out _);
            }

            runningCostSum -= window.RunningCosts[left];
            left++;
        }

        return window with { RunningCostSum = runningCostSum, Left = left };
    }

    // Evict every trailing index whose chargeTime this one dominates, so the deque
    // stays decreasing and its front is the window's maximum charge time.
    private static void PushMaxCandidate(int[] chargeTimes, RepoDeque maxWindow, int right)
    {
        while (maxWindow.TryPeekBack(out var backIndex) && chargeTimes[backIndex] <= chargeTimes[right])
        {
            maxWindow.TryPopBack(out _);
        }

        maxWindow.PushBack(right);
    }

    // The monotonic-deque sweep's whole running state: the problem's own fixed inputs
    // plus the current window's deque of candidate charge-time indices, its summed
    // running cost and its left edge.
    private readonly record struct BudgetWindow(
        int[] ChargeTimes,
        int[] RunningCosts,
        long Budget,
        RepoDeque MaxCandidates,
        long RunningCostSum,
        int Left);
}
