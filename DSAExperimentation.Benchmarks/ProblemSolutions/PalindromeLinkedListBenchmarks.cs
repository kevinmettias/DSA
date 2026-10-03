using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.PalindromeLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromeLinkedListSolution's, the same methods
// PalindromeLinkedListSolutionTests proves correct. [GlobalSetup] builds a palindrome-shaped
// list once and reuses it across iterations. The stack arm only reads Value/Next, and
// the fast/slow arm rewires the second half and then reverses it back before
// returning, so the cached list is left unchanged and stays valid across iterations
// (mirrors MiddleOfTheLinkedListBenchmarks' non-mutating precedent). The list is a
// genuine palindrome so every strategy walks the full list rather than
// short-circuiting on the first mismatch.
public class PalindromeLinkedListBenchmarks
{
    private const int RandomSeed = 234; // LC problem number
    private const int MaxHalfValueExclusive = 10; // LC 234: 0 <= Node.val <= 9

    private SinglyLinkedListNode<int>? _head;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = BuildPalindrome(new Random(RandomSeed), Length);

    private static SinglyLinkedListNode<int>? BuildPalindrome(Random random, int length)
    {
        var half = (length + 1) / 2;
        var values = new int[length];

        for (var i = 0; i < half; i++)
        {
            var value = random.Next(0, MaxHalfValueExclusive);
            values[i] = value;
            values[length - 1 - i] = value;
        }

        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    [Benchmark(Baseline = true)]
    public bool IsPalindromeByStackReversal() => PalindromeLinkedListSolution.IsPalindromeByStackReversal(_head);

    [Benchmark]
    public bool FastSlowReversal() => PalindromeLinkedListSolution.IsPalindromeByFastSlowReversal(_head);
}
