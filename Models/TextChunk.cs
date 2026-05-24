namespace MultiModalRagDemo.Models
{
    public class TextChunk

    {
        public int Index { get; set; }

        public string Content { get; set; } = string.Empty;

        public int CharacterCount => Content.Length;

    }
}
