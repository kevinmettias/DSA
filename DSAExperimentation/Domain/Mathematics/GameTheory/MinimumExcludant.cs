namespace DSAExperimentation.Domain.Mathematics.GameTheory;

// The minimum excludant of a set of natural numbers, written mex: the least natural number the set does
// not contain, so mex {} = 0 and mex {0, 1, 3} = 2. It is how a Grundy number is made from the numbers
// of the positions a move reaches. Among n numbers the mex is at most n, so only the numbers below n + 1
// need to be marked, and a small list marks them on the stack.
/// <summary>The minimum excludant: the least natural number a list of naturals does not contain.</summary>
public static class MinimumExcludant
{
    // Lists this long or shorter mark their numbers in a stack buffer; longer ones allocate.
    private const int StackMarkLimit = 256;

    /// <summary>Finds the least natural number the list does not contain.</summary>
    /// <param name="numbers">Natural numbers, in any order, repeats allowed.</param>
    /// <returns>The least natural number missing from <paramref name="numbers"/>.</returns>
    public static int Of(ReadOnlySpan<int> numbers)
    {
        var length = numbers.Length + 1;
        var seen = length <= StackMarkLimit ? stackalloc bool[length] : new bool[length];

        foreach (var number in numbers)
        {
            if ((uint)number < (uint)length)
            {
                seen[number] = true;
            }
        }

        return seen.IndexOf(false);
    }
}
