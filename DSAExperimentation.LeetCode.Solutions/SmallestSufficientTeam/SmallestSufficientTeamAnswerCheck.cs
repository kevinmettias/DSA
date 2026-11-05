namespace DSAExperimentation.LeetCode.Solutions.SmallestSufficientTeam;

// LC 1125 accepts any sufficient team of the smallest size, its people's indices in any order. The catalog
// records one, so an answer stands when it is as large as the recorded team, names each person at most
// once, and between them its people hold every required skill. The brute force and the bitmask memo meet
// teams of the smallest size in different orders.
internal static class SmallestSufficientTeamAnswerCheck
{
    public static bool IsAccepted(string[] reqSkills, string[][] people, int[] answer, int[] recorded)
    {
        var isEachPersonOnceAndReal = answer.Distinct().Count() == answer.Length && answer.All(person => person >= 0 && person < people.Length);
        return answer.Length == recorded.Length && isEachPersonOnceAndReal && reqSkills.All(skill => answer.Any(person => people[person].Contains(skill)));
    }
}
