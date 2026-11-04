namespace DSAExperimentation.LeetCode.Solutions.SpecialBinaryString;

// LC 761's promise about its input: the string is special: as many 1s as 0s, and no prefix with
// more 0s than 1s. LeetCode's judge never poses an input that breaks it. A string that is not
// special splits into parts that are not either, and the two sorts order them differently.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class SpecialBinaryStringPromise
{
    public static bool IsKeptBy(string specialString)
    {
        var balance = 0;

        foreach (var bit in specialString)
        {
            balance += bit == '1' ? 1 : -1;

            if (balance < 0)
            {
                return false;
            }
        }

        return balance == 0;
    }
}
