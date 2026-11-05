namespace DSAExperimentation.LeetCode.Solutions.StampingTheSequence;

// LC 936 accepts any sequence of stamp positions, at most 10 * target.length of them, that turns a string of
// '?' into the target, and the empty array only when none does. The catalog records one sequence, so an
// answer stands when it is empty exactly where the recorded one is, or else such a sequence. The two
// strategies peel stamps off the target in different orders.
internal static class StampingTheSequenceAnswerCheck
{
    // The statement's limit on turns, per letter of the target.
    private const int TurnsPerLetter = 10;

    public static bool IsAccepted(StampingTheSequenceSolution.StampPattern stamp, StampingTheSequenceSolution.TargetText target, int[] answer, int[] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        var lastStart = target.Text.Length - stamp.Text.Length;
        var isWithinTheRules = answer.Length <= TurnsPerLetter * target.Text.Length && answer.All(start => start >= 0 && start <= lastStart);
        return isWithinTheRules && StampedWith(stamp, target.Text.Length, answer) == target.Text;
    }

    // The string the stamps leave, each at its position in turn over the ones before it; a letter no stamp
    // reached stays '\0', which no target holds.
    private static string StampedWith(StampingTheSequenceSolution.StampPattern stamp, int length, int[] starts)
    {
        var letters = new char[length];

        foreach (var start in starts)
        {
            stamp.Text.CopyTo(0, letters, start, stamp.Text.Length);
        }

        return new string(letters);
    }
}
