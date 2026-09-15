namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayII;

// The Element LongestBalancedSubarrayIISolution's LazySegmentTree tracks at every
// node: the smallest and largest "distinct-odd-count minus distinct-even-count"
// running balance anywhere in that node's index range. A witness for LC 3721
// alone, in this problem's own folder rather than Domain/ - see
// OpenTheLockSolution's own precedent for a single-problem witness living beside
// its solution class (ARCHITECTURE.md 17.3).
internal readonly record struct BalanceRange(int Min, int Max);
