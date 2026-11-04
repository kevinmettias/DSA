namespace DSAExperimentation.LeetCode.Solutions.MaximumEmployeesToBeInvitedToAMeeting;

// LC 2127's promise about its input: no employee's favourite is themself. LeetCode's judge never
// poses an input that breaks it. A self-favourite is a cycle of one, which neither strategy seats.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class MaximumEmployeesToBeInvitedToAMeetingPromise
{
    public static bool IsKeptBy(int[] favorite)
        => Enumerable.Range(0, favorite.Length).All(employee => favorite[employee] != employee && IsEmployee(favorite, favorite[employee]));

    private static bool IsEmployee(int[] favorite, int id) => id >= 0 && id < favorite.Length;
}
