namespace DSAExperimentation.LeetCode.GuessTheWord;

// LC 843's notion of a "match": the number of positions at which two equal-length
// words hold the same character - the complement of Hamming distance, and the only
// information the judge ever hands back. Both the oracle (computing its answer) and
// the solver (filtering candidates that would have produced that same answer) ask
// this one question, so it is stated once here rather than in each of them.
//
// It stays in this problem folder rather than DataStructures/Graph/Hamming because
// it is not a graph shape: LC 843 never walks one-character mutations, it only
// scores a guess.
internal static class WordMatch
{
    public static int ExactPositionMatches(string first, string second)
    {
        var count = 0;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
            {
                count++;
            }
        }

        return count;
    }
}
