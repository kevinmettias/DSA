namespace DSAExperimentation.LeetCode.Solutions.ImplementRand10UsingRand7;

// LC 470's judge calls rand10() n times, n being the case's one input, and each call
// may return any integer in 1..10, so an answer stands when it holds n values, each
// in that range; uniformity is a claim about many cases, which no one answer shows.
// The recorded answer is one random draw among many, so it does not take part.
internal static class ImplementRand10UsingRand7AnswerCheck
{
    private const int Rand10Maximum = 10;

    public static bool IsAccepted(int callCount, int[] answer, int[] recorded) =>
        answer.Length == callCount && answer.All(value => value is >= 1 and <= Rand10Maximum);
}
