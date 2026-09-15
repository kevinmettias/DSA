namespace DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameI;

// LeetCode 3304. Find the K-th Character in String Game I: word starts as
// "a"; every round appends a copy of the current word with each character
// shifted to its cyclic successor (z wraps to a). Return the k-th character
// (1-indexed) once word is long enough to have one.
//
// k <= 500, so the naive simulation is already fast - both strategies are
// here to record the closed form the doubling structure hides, not because
// the brute force needs replacing.
internal static class FindTheKthCharacterInStringGameISolution
{
    private const int AlphabetSize = 26;

    // The textbook answer: build the actual string one round at a time -
    // shift every existing character and append the shifted copy - until it
    // reaches position k. Deliberately written without this repo's
    // primitives; the arm the closed form below has to agree with.
    public static char KthCharacterBySimulation(int k)
    {
        var word = new List<char> { 'a' };

        while (word.Count < k)
        {
            var roundLength = word.Count;

            for (var i = 0; i < roundLength; i++)
            {
                word.Add(NextChar(word[i]));
            }
        }

        return word[k - 1];
    }

    private static char NextChar(char c) => c == 'z' ? 'a' : Shifted(c);

    // The shift by one, before the wrap: 'a' -> 'b' through 'y' -> 'z'.
    private static char Shifted(char c) => (char)(c + 1);

    // Every round is "append a +1-shifted copy of the whole word so far", so
    // reaching position p (0-indexed) by repeated halving visits exactly the
    // rounds where p's binary representation has a 1 bit - each such round is
    // one where p fell in the shifted half rather than the untouched one. The
    // total shift applied to the seed 'a' is therefore popcount(p) mod the
    // alphabet size.
    public static char KthCharacterByBitCount(int k)
    {
        var shift = System.Numerics.BitOperations.PopCount((uint)(k - 1));
        return (char)('a' + shift % AlphabetSize);
    }
}
