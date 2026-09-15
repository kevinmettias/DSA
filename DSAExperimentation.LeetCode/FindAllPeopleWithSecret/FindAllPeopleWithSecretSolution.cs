using DSAExperimentation.DataStructures.KeyedDisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.FindAllPeopleWithSecret;

// LeetCode 2092. Find All People With Secret: person 0 and firstPerson know a
// secret before anything happens; a meeting shares it in both directions, and
// secrets propagate transitively within the SAME instant (a chain of meetings
// sharing one timestamp all resolve together) but never across timestamps in a
// single hop. The answer is every person who knows it once all meetings have run.
//
// MeetingSchedule does the grouping both strategies depend on; the strategies
// differ only in how they resolve one timestamp group.
internal static class FindAllPeopleWithSecretSolution
{
    // Person 0 holds the secret before any meeting happens - LC 2092's own initial
    // condition, alongside firstPerson.
    private const int SecretOrigin = 0;

    // The textbook answer: no hashing and no union-find, just rescan the group's
    // meetings and push "knows" across any edge that is still mixed, repeating
    // until a full pass changes nothing. O(k^2) per group in the worst case - a
    // chain needs up to k passes over k meetings. Deliberately written with BCL
    // parts only; it is the arm the composed strategy below has to justify itself
    // against.
    public static int[] FindAllPeopleByRepeatedRelaxation(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson)
    {
        var schedule = MeetingSchedule.Build(peopleCount, meetings, firstPerson);
        return FindAllPeopleByRepeatedRelaxation(schedule);
    }

    public static int[] FindAllPeopleByRepeatedRelaxation(MeetingSchedule schedule)
    {
        var knowsSecret = SeedKnownSecret(schedule);

        foreach (var group in schedule.TimeGroups)
        {
            var changed = true;

            while (changed)
            {
                changed = RelaxGroupOnce(group, knowsSecret);
            }
        }

        return CollectKnowers(knowsSecret);
    }

    private static bool RelaxGroupOnce((int First, int Second)[] group, bool[] knowsSecret)
    {
        var changed = false;

        foreach (var (first, second) in group)
        {
            if (knowsSecret[first] == knowsSecret[second])
            {
                continue;
            }

            knowsSecret[first] = true;
            knowsSecret[second] = true;
            changed = true;
        }

        return changed;
    }

    // This repo's own composition: a fresh KeyedDisjointSet<int> per timestamp
    // group over just that group's participants - the same "union on shared
    // membership" shape AccountsMerge uses for shared emails, applied to shared
    // timestamps instead - and a Set<int> marking which resulting component
    // representatives already had an informed member walking in. Near
    // O(k * a(k)) per group, no repeated rescanning.
    //
    // Building a fresh KeyedDisjointSet per timestamp (rather than one global
    // DisjointSet needing an "undo union" this repo has no primitive for) is what
    // keeps a same-timestamp-only chain from ever leaking a false connection into
    // a later timestamp's group.
    public static int[] FindAllPeopleByKeyedDisjointSet(
        int peopleCount, (int First, int Second, int Time)[] meetings, int firstPerson)
    {
        var schedule = MeetingSchedule.Build(peopleCount, meetings, firstPerson);
        return FindAllPeopleByKeyedDisjointSet(schedule);
    }

    public static int[] FindAllPeopleByKeyedDisjointSet(MeetingSchedule schedule)
    {
        var knowsSecret = SeedKnownSecret(schedule);

        foreach (var group in schedule.TimeGroups)
        {
            PropagateWithinTimestamp(group, knowsSecret);
        }

        return CollectKnowers(knowsSecret);
    }

    private static void PropagateWithinTimestamp((int First, int Second)[] group, bool[] knowsSecret)
    {
        var participants = CollectParticipants(group);
        var components = BuildComponents(group, participants);
        var secretRoots = FindSecretRoots(participants, components, knowsSecret);
        MarkQualifyingComponents(participants, components, secretRoots, knowsSecret);
    }

    private static List<int> CollectParticipants((int First, int Second)[] group)
    {
        var participants = new List<int>();

        foreach (var (first, second) in group)
        {
            participants.Add(first);
            participants.Add(second);
        }

        return participants;
    }

    private static KeyedDisjointSet<int> BuildComponents((int First, int Second)[] group, List<int> participants)
    {
        var components = new KeyedDisjointSet<int>(participants);

        foreach (var (first, second) in group)
        {
            components.TryUnion(first, second);
        }

        return components;
    }

    private static Set<int> FindSecretRoots(List<int> participants, KeyedDisjointSet<int> components, bool[] knowsSecret)
    {
        var secretRoots = new Set<int>();

        foreach (var person in participants)
        {
            if (knowsSecret[person] && components.TryFind(person, out var root))
            {
                secretRoots.TryAdd(root);
            }
        }

        return secretRoots;
    }

    private static void MarkQualifyingComponents(
        List<int> participants, KeyedDisjointSet<int> components, Set<int> secretRoots, bool[] knowsSecret)
    {
        foreach (var person in participants)
        {
            if (components.TryFind(person, out var root) && secretRoots.Has(root))
            {
                knowsSecret[person] = true;
            }
        }
    }

    private static bool[] SeedKnownSecret(MeetingSchedule schedule)
    {
        var knowsSecret = new bool[schedule.PeopleCount];
        knowsSecret[SecretOrigin] = true;
        knowsSecret[schedule.FirstPerson] = true;

        return knowsSecret;
    }

    private static int[] CollectKnowers(bool[] knowsSecret)
    {
        var people = new List<int>();

        for (var person = 0; person < knowsSecret.Length; person++)
        {
            if (knowsSecret[person])
            {
                people.Add(person);
            }
        }

        return people.ToArray();
    }
}
