using DSAExperimentation.LeetCode.FindTheShortestSuperstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheShortestSuperstringSolution's, the same methods
// FindTheShortestSuperstringSolutionTests proves correct - the textbook permutation brute
// force (O(n! * n)) against the bitmask-TSP DP built on this repo's own Memoizer
// (O(2^n * n^2)). The overlap matrix is the prepared input both hoisted overloads
// take, so it is built once per word count in [GlobalSetup] and neither arm is charged
// for the string-overlap preprocessing they share.
//
// Sizes are per arm. n! overtakes 2^n * n^2 fast enough that the brute force would
// dominate the run past 9 words, so it stops there; the bitmask DP runs on to LC 943's
// own bound of 12 words, and the two are compared at the counts both run.
//
// [GlobalSetup] builds a word chain rather than a random word list, because LC 943
// does not have one answer per input: several maximum-overlap orders can assemble to
// several different shortest superstrings, and the arms legitimately return different
// ones. The random list this benchmark used before did exactly that - at this seed it
// admitted four distinct shortest superstrings for six words and two for nine, with
// no seed in reach admitting only one - which made the two published numbers
// incomparable, the same defect AccountsMergeBenchmarks' shared-name generator had.
//
// The chain makes the answer unique. Each word after the first repeats its
// predecessor's trailing four characters and adds one new one, and every word's
// trailing window is kept distinct, so a pair can overlap by the maximum four only
// when it is a consecutive pair of the chain: the chain order is then the single
// highest-overlap tour and both arms have exactly one shortest superstring to return.
// That holds at every word count, the DP's larger ones included, since the chain is
// built the same way from the same seed whatever its length. Word length, alphabet and
// seed are unchanged, and the arms still compare the same two searches over the same
// prepared overlap matrix.
public class FindTheShortestSuperstringBenchmarks
{
    // LC problem number, reused as the deterministic word seed.
    private const int WordSeed = 943;
    private const int WordLength = 5;

    // The letters a word is drawn from, and so the width of each trailing window - lowercase,
    // as LC 943 requires.
    private const string Alphabet = "acgt";

    private Dictionary<int, WordOverlaps> _overlapsByWordCount = [];

    public static IEnumerable<int> PermutationSizes => [6, 9];

    public static IEnumerable<int> MemoizedBitmaskSizes => [.. PermutationSizes, 11, 12];

    // Every word count any arm runs has its chain built here, outside the timed region,
    // each from its own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _overlapsByWordCount = MemoizedBitmaskSizes.ToDictionary(
            wordCount => wordCount,
            wordCount => WordOverlaps.Build(BuildUniqueAnswerWords(wordCount, new Random(WordSeed))));

    private static string[] BuildUniqueAnswerWords(int wordCount, Random random)
    {
        var words = new List<string> { BuildWord(random) };

        // Both windows of the opening word are reserved: a later word repeating its
        // trailing window would add a second maximum-overlap link into the chain, and a
        // later word repeating its leading window would let an order run off the end of
        // the chain and still overlap by the maximum.
        var reservedWindows = new HashSet<string>
        {
            TrailingWindow(words[0]),
            LeadingWindow(words[0]),
        };

        while (words.Count < wordCount)
        {
            var successor = BuildSuccessorWord(words[^1], random, reservedWindows);

            words.Add(successor);
        }

        return [.. words];
    }

    private static string BuildWord(Random random)
    {
        var letters = new char[WordLength];

        for (var i = 0; i < WordLength; i++)
        {
            letters[i] = NextLetter(random);
        }

        return new string(letters);
    }

    // The window a word opens with: the first WordLength - 1 characters.
    private static string LeadingWindow(string word) => word[..(WordLength - 1)];

    private static string BuildSuccessorWord(
        string previous, Random random, HashSet<string> reservedWindows)
    {
        // Returns on the first letter whose candidate opens a trailing window no word has
        // claimed yet: Alphabet has four letters against a window of WordLength - 1, so
        // 256 windows are reachable and at most one more than the word count are ever reserved.
        while (true)
        {
            var candidate = TrailingWindow(previous) + NextLetter(random);

            if (reservedWindows.Add(TrailingWindow(candidate)))
            {
                return candidate;
            }
        }
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(PermutationSizes))]
    public string BruteForcePermutations(int wordCount) =>
        FindTheShortestSuperstringSolution.ShortestSuperstringByPermutations(_overlapsByWordCount[wordCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedBitmaskSizes))]
    public string MemoizedBitmaskDp(int wordCount) =>
        FindTheShortestSuperstringSolution.ShortestSuperstringByMemoizedBitmask(_overlapsByWordCount[wordCount]);

    private static char NextLetter(Random random) => Alphabet[random.Next(Alphabet.Length)];

    // The window a successor word has to repeat: the last WordLength - 1 characters.
    private static string TrailingWindow(string word) => word[(word.Length - WordLength + 1)..];
}
