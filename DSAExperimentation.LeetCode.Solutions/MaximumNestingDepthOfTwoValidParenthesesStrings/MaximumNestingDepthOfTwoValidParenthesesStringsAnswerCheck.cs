namespace DSAExperimentation.LeetCode.Solutions.MaximumNestingDepthOfTwoValidParenthesesStrings;

// LC 1111 accepts any split of the sequence into two valid parentheses strings, A marked 0 and B marked 1,
// that makes the deeper of the two as shallow as it can be. The catalog records one split, so an answer
// stands when it marks every character 0 or 1, both strings it makes are valid, and the deeper of them is
// no deeper than in the recorded split. Both strategies give the opposite marks to the recorded ones.
internal static class MaximumNestingDepthOfTwoValidParenthesesStringsAnswerCheck
{
    internal const char Open = '(';

    // The depth a split reports when one of its two strings closes a parenthesis it never opened or
    // leaves one open, deeper than any valid split.
    private const int Invalid = int.MaxValue;

    public static bool IsAccepted(string seq, int[] answer, int[] recorded)
    {
        var isMarkedThroughout = answer.Length == seq.Length && answer.All(mark => mark is 0 or 1);

        if (!isMarkedThroughout)
        {
            return false;
        }

        var depth = DeeperDepthOf(seq, answer);
        return depth != Invalid && depth <= DeeperDepthOf(seq, recorded);
    }

    // The deeper of the two strings' nesting depths, or Invalid when either string is not valid.
    private static int DeeperDepthOf(string seq, int[] marks)
    {
        var depths = new int[2];
        var deepest = 0;

        for (var index = 0; index < seq.Length; index++)
        {
            var isOpening = seq[index] == Open;
            depths[marks[index]] += isOpening ? 1 : -1;

            if (depths[marks[index]] < 0)
            {
                return Invalid;
            }

            deepest = Math.Max(deepest, depths[marks[index]]);
        }

        return depths.All(depth => depth == 0) ? deepest : Invalid;
    }
}
