namespace DSAExperimentation.LeetCode.Solutions.MeetingRoomsIII;

// LC 2402's promise about its input: every meeting starts before it ends, and no two start
// together. LeetCode's judge never poses an input that breaks it. Two meetings starting together
// have no order of booking, and the strategies each pick their own. Generated cases do not always
// keep the promise, so a harness that runs them asks this first.
internal static class MeetingRoomsIIIPromise
{
    public static bool IsKeptBy(int roomCount, int[][] meetings)
        => meetings.All(meeting => meeting[0] < meeting[1]) && meetings.Select(meeting => meeting[0]).Distinct().Count() == meetings.Length;
}
