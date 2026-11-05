namespace DSAExperimentation.LeetCode.Solutions.BeautifulArray;

// LC 932 accepts "any beautiful array" of length n: a permutation of 1 to n in which no value sits between
// two whose average it is (no i < k < j with 2 * nums[k] == nums[i] + nums[j]). So an answer stands when it
// is such a permutation; the recorded answer is one of them, so it does not take part. The backtracking and
// the divide-and-conquer strategies build different ones.
internal static class BeautifulArrayAnswerCheck
{
    public static bool IsAccepted(int length, int[] answer, int[] recorded)
    {
        var oneToLength = Enumerable.Range(1, length);

        return answer.Order().SequenceEqual(oneToLength) && !HasAverageBetween(answer);
    }

    // Whether some pair's average stands between them: found by the average's position, one pair at a time.
    private static bool HasAverageBetween(int[] values)
    {
        var positionOf = new int[values.Length + 1];

        for (var position = 0; position < values.Length; position++)
        {
            positionOf[values[position]] = position;
        }

        for (var left = 0; left < values.Length; left++)
        {
            for (var right = left + 1; right < values.Length; right++)
            {
                if (IsAverageBetween(values, positionOf, left, right))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsAverageBetween(int[] values, int[] positionOf, int left, int right)
    {
        var sum = values[left] + values[right];

        if (sum % 2 != 0)
        {
            return false;
        }

        var middle = positionOf[sum / 2];
        return middle > left && middle < right;
    }
}
