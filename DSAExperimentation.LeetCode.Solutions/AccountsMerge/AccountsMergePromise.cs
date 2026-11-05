namespace DSAExperimentation.LeetCode.Solutions.AccountsMerge;

// LC 721's promise about its input: two accounts that share an email belong to one person, so they carry
// one name. LeetCode's judge never poses an input that breaks it. Accounts joined by an email but named
// differently leave the merged account no single name, and the strategies each keep a different one.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class AccountsMergePromise
{
    public static bool IsKeptBy(string[][] accounts)
    {
        var nameByEmail = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var account in accounts)
        {
            foreach (var email in account.Skip(1))
            {
                if (nameByEmail.TryGetValue(email, out var name) && name != account[0])
                {
                    return false;
                }

                nameByEmail[email] = account[0];
            }
        }

        return true;
    }
}
