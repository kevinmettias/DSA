namespace DSAExperimentation.Domain.Devices.Telephone;

// The letters on a telephone keypad's keys, as ITU-T E.161 assigns them and as LC 17 reads them:
// 2 is abc, 3 def, 4 ghi, 5 jkl, 6 mno, 7 pqrs, 8 tuv and 9 wxyz, and 0 and 1 carry none. Keypads
// print the letters in capitals, but LC 17's strings are lowercase, so the member says lowercase.
// These are the problem's *what* (ARCHITECTURE §17.6); how a strategy forms
// combinations from them stays with it, and the baseline keeps its own copy (§17.5).
//
// A key's letters are read from a table by its digit rather than chosen by a switch: a search reads
// them once per node, and the JIT can judge a switch too big to inline there (§17.10).
/// <summary>The letters on each key of a telephone keypad.</summary>
public static class PhoneKeypad
{
    // Each key's letters, at the key's digit.
    private static readonly string[] LettersByKey = ["", "", "abc", "def", "ghi", "jkl", "mno", "pqrs", "tuv", "wxyz"];

    /// <summary>The lowercase letters on a keypad key, in the order the key lists them.</summary>
    /// <param name="key">The key's character, '0' through '9'.</param>
    /// <returns>The key's letters; empty for '0' and '1', which carry none, and for a character that is no digit key.</returns>
    public static string LowercaseLettersOf(char key)
    {
        var digit = (uint)(key - '0');

        if (digit >= (uint)LettersByKey.Length)
        {
            return string.Empty;
        }

        return LettersByKey[digit];
    }
}
