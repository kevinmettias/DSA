namespace DSAExperimentation.LeetCode.FindAllPeopleWithSecret;

// LC 2092's input reshaped once into the form every strategy walks: the meetings
// bucketed by timestamp, buckets in ascending time order, plus the two facts that
// seed the walk - how many people exist, and which one starts out knowing the
// secret alongside person 0.
//
// Grouping is what makes the problem's "same instant" rule expressible at all: a
// secret propagates transitively within one timestamp group and never across two
// in a single hop, so the group is the unit of work each strategy iterates.
//
// Bespoke to this one problem, which is why it lives beside the solution rather
// than in Domain (§17.3): nothing here fixes a modulus or a vertex set another
// problem could share - it is simply this problem's own input shape, hoisted out
// of the measured strategies so a benchmark can build it once in [GlobalSetup].
internal readonly record struct MeetingSchedule(
    int PeopleCount,
    (int First, int Second)[][] TimeGroups,
    int FirstPerson)
{
    public static MeetingSchedule Build(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson)
    {
        var timeGroups = meetings
            .GroupBy(meeting => meeting.Time)
            .OrderBy(group => group.Key)
            .Select(group => group.Select(meeting => (meeting.First, meeting.Second)).ToArray())
            .ToArray();

        return new MeetingSchedule(peopleCount, timeGroups, firstPerson);
    }
}
