namespace DSAExperimentation.LeetCode.Solutions.DecodeString;

// LC 394's promise about its input: the string is a valid encoding, its brackets balanced and every
// '[' opened by the repeat count before it, which is the only place a digit appears. LeetCode's judge
// never poses an input that breaks it. The recursive descent stops at a bracket no count opens, where
// the stack scan reads on. Generated cases do not always keep the promise, so a harness that runs them
// asks this first.
internal static class DecodeStringPromise
{
    private const char Open = '[';
    private const char Close = ']';

    public static bool IsKeptBy(string encoded) => IsBalanced(encoded) && IsEachCountBeforeABracket(encoded);

    // No ']' closes more than has opened, and none is left open at the end.
    private static bool IsBalanced(string encoded)
    {
        var depth = 0;

        foreach (var character in encoded)
        {
            depth += character switch
            {
                Open => 1,
                Close => -1,
                _ => 0,
            };

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }

    // Each '[' follows a digit, and each digit is followed by another digit or a '['.
    private static bool IsEachCountBeforeABracket(string encoded)
        => Enumerable.Range(0, encoded.Length).All(index => encoded[index] switch
        {
            Open => index > 0 && char.IsAsciiDigit(encoded[index - 1]),
            _ when char.IsAsciiDigit(encoded[index]) => index + 1 < encoded.Length && (char.IsAsciiDigit(encoded[index + 1]) || encoded[index + 1] == Open),
            _ => true,
        });
}
