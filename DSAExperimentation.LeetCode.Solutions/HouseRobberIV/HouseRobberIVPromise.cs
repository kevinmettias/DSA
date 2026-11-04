namespace DSAExperimentation.LeetCode.Solutions.HouseRobberIV;

// LC 2560's promise about its input: the robber can always take k houses with none adjacent, so k
// is at most (n + 1) / 2. LeetCode's judge never poses an input that breaks it. The linear scan
// assumes some capability takes k houses. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class HouseRobberIVPromise
{
    public static bool IsKeptBy(int[] nums, int requiredHouseCount)
        => requiredHouseCount >= 1 && requiredHouseCount <= (nums.Length + 1) / 2;
}
