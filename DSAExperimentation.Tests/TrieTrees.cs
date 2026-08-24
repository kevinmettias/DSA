namespace DSAExperimentation.Tests;

public static class TrieTrees
{
    public static TrieNode FromWords(params string[] words)
    {
        var root = new TrieNode();

        foreach (var word in words)
        {
            var current = root;

            foreach (var ch in word)
            {
                var index = ch - 'a';
                current = current.Children[index] ??= new TrieNode();
            }

            current.IsWord = true;
        }

        return root;
    }
}
