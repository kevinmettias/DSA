using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountAnagrams;

// LeetCode 2514. Count Anagrams: split s into words on ' '; the number of "good"
// strings is the product, over every word, of that word's distinct-letter-
// permutation count (the multinomial coefficient word.Length! divided by the
// factorial of each letter's own repeat count), reported modulo 1e9+7. Both
// strategies answer the same question with the same signature so the test harness
// can assert they agree and the benchmark harness can time them against each other
// without either restating the algorithm (TwoSumSolution precedent).
internal static class CountAnagramsSolution
{
    private const int AlphabetSize = 26;

    // Textbook baseline: recursively enumerates every permutation of each word into
    // a HashSet<string> to dedupe, then multiplies the per-word counts (reduced mod
    // 1e9+7 after every word, the same running-product LeetCode itself asks for, so
    // this never overflows on a many-word sentence). Correct, but factorial-time per
    // word - the arm the modular-factorial strategy has to beat.
    public static long CountAnagramsByBruteForce(string s)
    {
        var answer = 1L;

        foreach (var word in s.Split(' '))
        {
            answer = answer * DistinctPermutationsByBruteForce(word) % ModularArithmetic.Modulo;
        }

        return answer;
    }

    // One O(maxWordLength) factorial/inverse-factorial table build (this repo's own
    // ModularArithmetic supplies the modulus and the Fermat's-little-theorem
    // inverse the table needs - the same shape
    // RoomWaysPrecomputedFactorialAlgebra already uses for a different counting
    // problem), then O(word.Length) per word: factorial[len] times the inverse
    // factorial of each letter's repeat count, mod 1e9+7. No permutation is ever
    // materialized.
    public static long CountAnagramsByModularFactorial(string s)
    {
        var words = s.Split(' ');
        var maxLength = 0;

        foreach (var word in words)
        {
            maxLength = Math.Max(maxLength, word.Length);
        }

        var (factorial, inverseFactorial) = BuildFactorialTable(maxLength);
        var answer = 1L;

        foreach (var word in words)
        {
            answer = answer * DistinctPermutationsByModularFactorial(word, factorial, inverseFactorial) % ModularArithmetic.Modulo;
        }

        return answer;
    }

    private static long DistinctPermutationsByBruteForce(string word)
    {
        var distinct = new HashSet<string>();
        Permute(word.ToCharArray(), 0, distinct);
        return distinct.Count;
    }

    private static void Permute(char[] chars, int start, HashSet<string> distinct)
    {
        if (start == chars.Length)
        {
            distinct.Add(new string(chars));
            return;
        }

        for (var i = start; i < chars.Length; i++)
        {
            (chars[start], chars[i]) = (chars[i], chars[start]);
            Permute(chars, start + 1, distinct);
            (chars[start], chars[i]) = (chars[i], chars[start]);
        }
    }

    private static long DistinctPermutationsByModularFactorial(string word, long[] factorial, long[] inverseFactorial)
    {
        var counts = new int[AlphabetSize];

        foreach (var c in word)
        {
            counts[c - 'a']++;
        }

        var result = factorial[word.Length];

        foreach (var count in counts)
        {
            result = result * inverseFactorial[count] % ModularArithmetic.Modulo;
        }

        return result;
    }

    private static (long[] Factorial, long[] InverseFactorial) BuildFactorialTable(int maxLength)
    {
        var factorial = new long[maxLength + 1];
        var inverseFactorial = new long[maxLength + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxLength; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxLength] = ModularArithmetic.Inverse(factorial[maxLength]);

        for (var i = maxLength - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        return (factorial, inverseFactorial);
    }
}
