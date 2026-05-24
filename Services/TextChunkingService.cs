using MultimodalRagDemo.Services.Interfaces;
using MultiModalRagDemo.Models;

namespace MultimodalRagDemo.Services
{
    public class TextChunkingService : ITextChunkingService
    {
        public List<TextChunk> ChunkText(
            string text,
            int chunkSize = 800,
            int overlapSize = 100)
        {
            var chunks = new List<TextChunk>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            if (chunkSize <= 0)
            {
                throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));
            }

            if (overlapSize < 0)
            {
                throw new ArgumentException("Overlap size cannot be negative.", nameof(overlapSize));
            }

            if (overlapSize >= chunkSize)
            {
                throw new ArgumentException("Overlap size must be smaller than chunk size.", nameof(overlapSize));
            }

            string normalizedText = NormalizeWhitespace(text);

            int startIndex = 0;
            int chunkIndex = 1;

            while (startIndex < normalizedText.Length)
            {
                int remainingLength = normalizedText.Length - startIndex;
                int currentChunkSize = Math.Min(chunkSize, remainingLength);

                string chunkContent = normalizedText
                    .Substring(startIndex, currentChunkSize)
                    .Trim();

                if (!string.IsNullOrWhiteSpace(chunkContent))
                {
                    chunks.Add(new TextChunk
                    {
                        Index = chunkIndex,
                        Content = chunkContent
                    });

                    chunkIndex++;
                }

                if (startIndex + currentChunkSize >= normalizedText.Length)
                {
                    break;
                }

                startIndex += chunkSize - overlapSize;
            }

            return chunks;
        }

        private static string NormalizeWhitespace(string text)
        {
            return string.Join(
                " ",
                text.Split(
                    new[] { ' ', '\r', '\n', '\t' },
                    StringSplitOptions.RemoveEmptyEntries));
        }
    }
}

